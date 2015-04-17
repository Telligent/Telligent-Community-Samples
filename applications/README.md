# Polling Application
###What is the polling application?
The polling application example was created for the 2012 Big Social conference.  It demonstrates a complete end-to-end sample application using the Community Server API, plugins, widgets and more.

####How do I install this example?
1. Open the source code in Visual Studio 2010 or later.
2. Correct the project references by updating them to point to the correct Community Server assemblies.
3. Build the solution.
4. Copy the resulting Telligent.BigSocial.Polling.dll from the bin/ folder into the bin/ folder of a Community Server development installation.
5. Go to Control Panel > Administration > Site Administration > Site Configuration > Manage Plugins
6. Click the Configure button associated to the "Polling Application" plugin and provide a database connection string to an existing SQL Server database as a user with db_owner permissions (it does not need to be the Community Server database).  Save the configuration.
7. Enable the "Polling Application" and "Poll Activity" plugins by clicking the check boxes next to these plugins and clicking Save at the bottom of the Manage Plugins page.

The Polling application is now active within the site and a new poll can be created in all groups via the "New Post" button on the Group Application Navigation widget.

####Is there any additional documentation?
Yes, please see the [Polling Application](https://github.com/Telligent/Sample-Applications/wiki/Polling-Application) page in our wiki.

####How do I report a bug?
You can use the [issues section](https://github.com/Telligent/Sample-Applications/issues) of this repository to report any issues.

####Where can I ask questions?
Please visit our [developer community](http://community.zimbra.com/developers/f) to ask questions, get answers, collaborate and connect with other developers. Plus, give us feedback there so we can continue to improve these tools for you.

####Can I contribute?
Yes, we would love any contributions to this project.