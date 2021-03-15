namespace Winkle

open Pulumi
open Pulumi.FSharp

open Pulumi.Aws
open Pulumi.Aws.S3
open Pulumi.Aws.S3.Inputs
open Pulumi.Aws.Route53
open Pulumi.Aws.Route53.Inputs
open Pulumi.Aws.CloudFront
open Pulumi.Aws.CloudFront.Inputs
open Pulumi.Aws.Acm

open Winkle.FSharp

module Program =
  let specification () =
    let localConfig = Pulumi.Config()
    let domain = localConfig.Require(Config.domain)
    let awsConfig = Pulumi.Config("aws")
    let profile = awsConfig.Require("profile")

    let useast1 =
      Provider(
        Region.USEast1.ToString(),
        ProviderArgs(Profile = input profile, Region = input (Region.USEast1.ToString()))
      )

    // Provision a certificate for the domain (in us-east-1)
    let certificate =
      Certificate(
        domain,
        CertificateArgs(
          DomainName = input domain,
          SubjectAlternativeNames = inputList [ input ("www." + domain) ],
          ValidationMethod = input "DNS"
        ),
        CustomResourceOptions(Provider = useast1)
      )

    // Provision a zone
    let zoneArgs = ZoneArgs(Name = input domain)
    let zone = Zone(domain, zoneArgs)

    // Validate the certificate using DNS
    let certificateValidationRecords =
      [ Record(
          domain + "_1",
          RecordArgs(
            ZoneId = io zone.ZoneId,
            AllowOverwrite = input true,
            Name = io (certificate.DomainValidationOptions.Apply(fun options -> options.Item(0).ResourceRecordName)),
            Type =
              ioUnion1Of2 (certificate.DomainValidationOptions.Apply(fun options -> options.Item(0).ResourceRecordType)),
            Ttl = input 60,
            Records =
              inputList [ io (
                            certificate.DomainValidationOptions.Apply
                              (fun options -> options.Item(0).ResourceRecordValue)
                          ) ]
          )
        )
        Record(
          domain + "_2",
          RecordArgs(
            ZoneId = io zone.ZoneId,
            AllowOverwrite = input true,
            Name = io (certificate.DomainValidationOptions.Apply(fun options -> options.Item(1).ResourceRecordName)),
            Type =
              ioUnion1Of2 (certificate.DomainValidationOptions.Apply(fun options -> options.Item(1).ResourceRecordType)),
            Ttl = input 60,
            Records =
              inputList [ io (
                            certificate.DomainValidationOptions.Apply
                              (fun options -> options.Item(1).ResourceRecordValue)
                          ) ]
          )
        ) ]

    let certificateValidation =
      CertificateValidation(
        domain,
        CertificateValidationArgs(
          CertificateArn = io certificate.Arn,
          ValidationRecordFqdns =
            inputList (
              certificateValidationRecords
              |> List.map (fun record -> io record.Fqdn)
            )
        ),
        CustomResourceOptions(Provider = useast1)
      )

    // Provision a bucket to host the static website in
    let staticSiteBucket =
      Bucket(
        domain,
        BucketArgs(
          Acl = inputUnion2Of2 CannedAcl.PublicRead,
          BucketName = input domain,
          CorsRules =
            inputList [ input (
                          BucketCorsRuleArgs(
                            AllowedHeaders = inputList [ input "*" ],
                            AllowedMethods = inputList [ input "HEAD"; input "GET" ],
                            AllowedOrigins = inputList [ input ("https://" + domain) ],
                            ExposeHeaders = inputList [ input "ETag" ],
                            MaxAgeSeconds = input 3600
                          )
                        ) ],
          Website = input (BucketWebsiteArgs(IndexDocument = input "home.html"))
        )
      )

    // Distribute that website / bucket to all edges of the world

    let distribution =
      Distribution(
        domain,
        DistributionArgs(
          Enabled = input true,
          HttpVersion = input "http2",
          IsIpv6Enabled = input true,
          PriceClass = input "PriceClass_All",
          ViewerCertificate =
            input (
              DistributionViewerCertificateArgs(
                AcmCertificateArn = io certificateValidation.CertificateArn,
                MinimumProtocolVersion = input "TLSv1.2_2019",
                SslSupportMethod = input "sni-only"
              )
            ),
          DefaultRootObject = input "home.html",
          Aliases =
            inputList [ input domain
                        input ("www." + domain) ],
          DefaultCacheBehavior =
            input (
              DistributionDefaultCacheBehaviorArgs(
                AllowedMethods =
                  inputList [ input "HEAD"
                              input "GET"
                              input "OPTIONS" ],
                CachedMethods =
                  inputList [ input "HEAD"
                              input "GET"
                              input "OPTIONS" ],
                Compress = input true,
                ForwardedValues =
                  input (
                    DistributionDefaultCacheBehaviorForwardedValuesArgs(
                      Cookies =
                        input (DistributionDefaultCacheBehaviorForwardedValuesCookiesArgs(Forward = input "none")),
                      QueryString = input false
                    )
                  ),
                DefaultTtl = input 3600,
                MaxTtl = input 86400,
                MinTtl = input 300,
                TargetOriginId = io staticSiteBucket.Arn,
                ViewerProtocolPolicy = input "redirect-to-https"
              )
            ),
          Origins =
            inputList [ input (
                          DistributionOriginArgs(
                            OriginId = io staticSiteBucket.Arn,
                            DomainName = io staticSiteBucket.WebsiteEndpoint,
                            CustomOriginConfig =
                              input (
                                DistributionOriginCustomOriginConfigArgs(
                                  OriginProtocolPolicy = input "http-only",
                                  HttpPort = input 80,
                                  HttpsPort = input 443,
                                  OriginSslProtocols = inputList [ input "TLSv1.2" ]
                                )
                              )
                          )
                        ) ],
          Restrictions =
            input (
              DistributionRestrictionsArgs(
                GeoRestriction = input (DistributionRestrictionsGeoRestrictionArgs(RestrictionType = input "none"))
              )
            ),
          WaitForDeployment = input true
        )
      )

    let distributionAliasRecords =
      [ Record(
          domain,
          RecordArgs(
            ZoneId = io zone.ZoneId,
            Name = input domain,
            Type = inputUnion2Of2 RecordType.A,
            Aliases =
              inputList [ input (
                            RecordAliasArgs(
                              Name = io distribution.DomainName,
                              ZoneId = io distribution.HostedZoneId,
                              EvaluateTargetHealth = input true
                            )
                          ) ]
          )
        )
        Record(
          ("www." + domain),
          RecordArgs(
            ZoneId = io zone.ZoneId,
            Name = input ("www." + domain),
            Type = inputUnion2Of2 RecordType.A,
            Aliases =
              inputList [ input (
                            RecordAliasArgs(
                              Name = io distribution.DomainName,
                              ZoneId = io distribution.HostedZoneId,
                              EvaluateTargetHealth = input true
                            )
                          ) ]
          )
        ) ]

    dict [ ("staticSiteBucket.WebsiteDomain", staticSiteBucket.WebsiteDomain :> obj)
           ("staticSiteBucket.WebsiteEndpoint", staticSiteBucket.WebsiteEndpoint :> obj)
           ("staticSiteBucket.BucketDomainName", staticSiteBucket.BucketDomainName :> obj)
           //("staticSiteBucket.Website", staticSiteBucket.Website :> obj)
            ]

  [<EntryPoint>]
  let main _ = Deployment.run specification
