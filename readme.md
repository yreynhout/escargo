# Purpose

This repository contains a work around that allows people to join our Slack community without them getting frustrated about expiring invitation links.

## Shell

The front end, consisting of html, [Tailwind](https://tailwindcss.com/) css, and javascript. It embeds Google's [reCAPTHCA](https://www.google.com/recaptcha/about/) 3 in an attempt to mitigate abuse. For analytics, we're using [Plausible](https://plausible.io), GDPR friendly and cookieless. Shell is an executable, using F# and a HTML DSL, that generates a large portion of what is essentially a static website.

## Tail

The back end, consisting of an AWS Lambda Function, that automates and coordinates the process of joining of our Slack community. 

### Bone

The core logic of the back end, undone from AWS Lambda Function specifics, that integrates with Slack and Google Recaptcha.

### Helix

Tests ... testing the Bone.

## Winkle

Describes the required AWS infrastructure as code using Pulumi and F#.

## Acknowledgements

- The slack icon was taken from the Slack MediaKit (https://slack.com/intl/en-be/media-kit)
- The site background is "Composition 8" by Vassily Kandinsky and was taken from Wikimedia Commons (https://commons.wikimedia.org/wiki/File:Vassily_Kandinsky,_1923_-_Composition_8,_huile_sur_toile,_140_cm_x_201_cm,_Mus%C3%A9e_Guggenheim,_New_York.jpg)