#Table of Contents plugin for Telligent Community

The Table of Contents plugin  allows you to automatically generate a table of contents
for a wiki page, blog post, forum post or media file.

##Using the plugin

When authoring a long article insert the text "[toc]" (without the quotes)
towards the top of your article.  This will be replaced by an automatically
generated table of contents when users view your content.

To tell the addon how to generate the table of contents, you should use the
rich text editor to format the headings in your article to use the "Heading 2",
"Heading 3" etc. formats.


##Installation

Copy the compiled plugin to the bin directory of your community website, and the Job Server and enable the Table of Contents plugin. In 9.0 and above, that can be done at Administration > Extensions.  In 8.5 and below go to Control Panel > System Administration > Manage Plugins)


##Building Source Code

1. In the root of the project, Create a `TelligentEvolutionBinaries` directory
2. Copy the binaries from the /bin/ directory of your Telligent Community into the directory you just created
3. Open up Visual Studio and build the solution.
