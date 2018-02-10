# Setup
* Clone this repo
* Create `appsettings.development.json`
* Copy contents of `appsettings.json` (at least the empty settings) into `appsettings.development.json`
* Populate `appsettings.development.json`
  * Example connection strings:
    * local: `Server=.;Database=Jellypic;Integrated Security=SSPI;MultipleActiveResultSets=true`
    * Azure: get from Azure Portal
* Open Package Manager Console
* Run database migrations by executing this command: `update-database`

# Development
* Clone [jellypick-dashboard](https://github.com/johnnyoshika/jellypic-dashboard) and use it for UI.

# EF Migration
* After making changes to entities, run the following command in package manager console:
* `add-migration <migration name>`
* `update-database`
