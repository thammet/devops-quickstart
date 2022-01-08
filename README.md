# About 

DevopsQuickstart is a [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for creating and linking a codebase to Azure DevOps.

What it does:
* Create a repository on Azure DevOps
* Create and link [build pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/get-started/what-is-azure-pipelines?view=azure-devops) via any yml files it discovers in the codebase 


# How to use DevopsQuickstart

## Installation

`install nuget package command`


## Granting DevopsQuickstart Access 

DevopsQuickstart uses Microsoft Azure's [device code](https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-authentication-flows#device-code) authentication flow. 

> Basically, DevopsQuickstart will display a code that must then be entered at https://microsoft.com/devicelogin. Once the code is entered, the user chooses which account to grant DevopsQuickstart access to.


You must create an App Registration in the Azure Portal that gives permission to use the DevOps API. 

1. Navigate to the Microsoft identity platform for developers [App registrations](https://go.microsoft.com/fwlink/?linkid=2083908) page.
2. Select **New registration**.
3. In the **Register an application** page that appears, enter your application's registration information:
    * In the Name section, enter a meaningful application name that will be displayed to users of the app, for example `DevopsQuickstart-DeviceCodeFlow`.
    * Under **Supported account types**, select `Accounts in this organizational directory only`.
4. Select Register to create the application.
5. In the app's registration screen, find and note the Application (client) ID. You use this value in your app's configuration file(s) later in your code.
    * In the Advanced settings | Default client type section, flip the switch for `Treat application as a public client` to Yes.
6. Select **Save** to save your changes.
7. In the app's registration screen, select the **API permissions** blade in the left to open the page where we add access to the APIs that your application needs.
    * Select the **Add a permission** button and then,
    * Ensure that the **Microsoft APIs** tab is selected.
    * In the list of APIs, select the API `Azure DevOps`.
    * In the **Delegated permissions** section, select the `user_impersonation` in the list. Use the search box if necessary.
    * Select the **Add permissions** button at the bottom.

## Running the tool 

`devops-quickstart -t [YOUR_TENANT_ID] -c [YOUR_APP_REGISTRATION_CLIENT_ID] -o [YOUR_ORGANIZATION_URL]`

# Getting Custom 

DevopsQuickstart is open source. Feel free to download the source code and modify it to fit your needs!