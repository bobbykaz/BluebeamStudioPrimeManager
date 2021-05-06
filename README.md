# Overview
This was the publicly available integration "Prime Admin Toolkit" that utilized Bluebeam Inc.'s Studio API. This app was accessible between 2020-2021. It conformed to all of Bluebeam's branding, logging, and workflow requirements to be a publicly approved Integration. I have since stopped hosting the app, but it could be a good starting point for someone else who wants to make a Bluebeam Studio Prime Integration. See https://developers.bluebeam.com for info on how to sign up to use the API.

# Functionality
This integration provided Bluebeam Studio Prime administrators the capability to audit all of the data of all members belonging to their Prime. Current functionality was for managing user permissions systemm wide, and seeing general data on collaborations (Studio Projects and Sessions).

# Configuring
1. Modify `appsettings.json` (or provide an environment specific one depending on how you debug / publish) to include your client ID and client secret. When registering your integration with Bluebeam, your registered Redirect URI should be [http|https]://[your-hosted-domain]/auth-landing. For example, to run and debug locally you will want to specify `http://localhost/auth-landing`. If you will deploy this app to a separate path, you may want to change the `ClientRedirectPath` value as well.

2. Modify Resources.resx to include your app name / support email / website / etc

3. If you wish to use different scopes other than the default `full_prime` for this app, see ln 79 of Startup.cs. Note: This app was designed to be used by a Prime Administrator only, since administrators have full permissions to all Sessions and Projects in their Prime. Allowing non-admins to use this app could subject certain situations to failure due to the app not checking permissions in the current implementation.

4. Configuring LetsEncrypt: I used the Fluffyspoons LetsEncrypt package just to simplify hosting in AWS (one instance on ElasticBeanstalk, no loadbalancer to manage certs). You can disable LetsEncrypt in this app's `appsettings` if you are going to host this behind a loadbalancer or via another mechanism that will manage certs for you. Otherwise be sure to properly configure all the necessary LetsEncrypt configs.