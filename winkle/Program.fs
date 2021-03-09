﻿namespace Winkle

open Pulumi.FSharp

open Pulumi.Aws.S3
open Pulumi.Aws.S3.Inputs
open Pulumi.Aws.Route53
open Pulumi.Aws.CloudFront
open Pulumi.Aws.CloudFront.Inputs
open Pulumi.Aws.Acm

open Winkle.FSharp

module Program =
    let specification () =
      let config = Pulumi.Config()
      let domain = config.Require(Config.domain)

      // Provision a certificate for the domain
      let certificateArgs =
          CertificateArgs(
              DomainName = input domain,
              SubjectAlternativeNames = inputList [ input ("www." + domain) ],
              ValidationMethod = input "DNS"
          )

      let certificate = Certificate(domain, certificateArgs)

      // Provision a zone
      let zoneArgs = ZoneArgs(Name = input domain)
      let zone = Zone(domain, zoneArgs)

      // Validate the certificate using DNS
      let certificateValidationRecords =
          [ Record(
              domain + "_1",
              RecordArgs(
                  ZoneId = io zone.ZoneId,
                  Name =
                      io (
                          certificate.DomainValidationOptions.Apply(fun options -> options.Item(0).ResourceRecordName)
                      ),
                  Type =
                      ioUnion1Of2 (
                          certificate.DomainValidationOptions.Apply(fun options -> options.Item(0).ResourceRecordType)
                      ),
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
                    Name =
                        io (
                            certificate.DomainValidationOptions.Apply
                                (fun options -> options.Item(1).ResourceRecordName)
                        ),
                    Type =
                        ioUnion1Of2 (
                            certificate.DomainValidationOptions.Apply
                                (fun options -> options.Item(1).ResourceRecordType)
                        ),
                    Ttl = input 60,
                    Records =
                        inputList [ io (
                                        certificate.DomainValidationOptions.Apply
                                            (fun options -> options.Item(1).ResourceRecordValue)
                                    ) ]
                )
            ) ]

      let certificationValidation =
          CertificateValidation(
              domain,
              CertificateValidationArgs(
                  CertificateArn = io certificate.Arn,
                  ValidationRecordFqdns =
                      inputList (
                          certificateValidationRecords
                          |> List.map (fun record -> io record.Fqdn)
                      )
              )
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
                                          AllowedMethods =
                                              inputList [ input "HEAD"
                                                          input "GET" ],
                                          AllowedOrigins = inputList [ input ("https://" + domain) ],
                                          ExposeHeaders = inputList [ input "ETag" ],
                                          MaxAgeSeconds = input 3600
                                      )
                                  ) ],
                  Website = input (BucketWebsiteArgs(IndexDocument = input "home.html"))
              )
          )

      // Distribute that website / bucket to all edges of the world
      let s3OriginId = "S3-" + domain

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
                          AcmCertificateArn = io certificate.Arn,
                          CloudfrontDefaultCertificate = input true,
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
                          DefaultTtl = input 3600,
                          MaxTtl = input 86400,
                          MinTtl = input 300,
                          TargetOriginId = input s3OriginId,
                          ViewerProtocolPolicy = input "redirect-to-https"
                      )
                  ),
              Origins = inputList [
                  input (
                    DistributionOriginArgs(
                      DomainName = io staticSiteBucket.WebsiteDomain,
                      OriginId = input s3OriginId
                    )
                  )
              ],
              Restrictions = 
                input (
                    DistributionRestrictionsArgs(
                        GeoRestriction = 
                            input (
                                DistributionRestrictionsGeoRestrictionArgs(
                                    RestrictionType = input "none"
                                )
                            )
                    )
                ),
              WaitForDeployment = input true
          )
        )
      
      dict [
          ("staticSiteBucket.WebsiteDomain", staticSiteBucket.WebsiteDomain :> obj)
      ]

    [<EntryPoint>]
    let main _ = Deployment.run specification
