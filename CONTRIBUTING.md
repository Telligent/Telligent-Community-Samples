#Contributing
1. Fork the repository locally to perform all your work
2. Once completed, create a [PULL](https://help.github.com/articles/using-pull-requests/) Request with your changes. These will be reviewed and merged by one of the project owners. (NOTE: Project contributors can commit directly without pull requests but please continue to follow the guidelines below)

>By submitting a pull request or comitting to this repository you agree to release your source under the [MIT](http://opensource.org/licenses/MIT) license and that anyone can use, distribute or manipulate the code to suit their needs.  You also acknowledge that you have the right to distribute this code freely.


###Guidelines For Contributing Widgets
Everything must be included in its own folder under the /widgets directory (the directory of this README) containing the following:


1.  You must include a **README.md** file in the root of the your widget directory explaining the following:
  - What version of Community Server this should work on(e.g. Community Server 8.5 and higher)
  - A description of what the widget does
  - Any special installation instructions aside from importing the widget XML
  - If your widget requires 3rd party libraries, JQuery plugins etc, please list them.  They must be compatible with the [MIT](http://opensource.org/licenses/MIT) license.
2. The XML export of the widget to be imported into Community Server.
3. If your widget requires custom code, a compiled version of the DLL(s).
4. If your widget requires custom code, the source code in its own folder in the parent folder labeled **"source"** or **"src"**.  **DO NOT** include any Community Server libraries(DLLs).  Provide instructions on how to add them to the source from an existing Community Server installation.
5. All custom code must use supported Community Server APIs.


