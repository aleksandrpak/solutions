# TextIndexing

Test project for indexing text files.

## Configuring:
### Service arguments: 
  -t, --port               (Default: 1234) A port to listen for incoming
                           requests

  -l, --parser-library     A full path to DLL containing IQueryParser
                           implementation

  -p, --parser-type        An IQueryParser implementation type name

  -r, --indexer-library    A full path to DLL containing IFileIndexer implementation

  -i, --indexer-type       An IFileIndexer implementation type name

  --help                   Display this help screen.

### Using advanced query parser:
*Service* Project properties -> Debug -> Command Line Arguments: *-l "../../../AdvancedQueryParser/bin/{Debug/Release}/AdvancedQueryParser.dll" -p "TextIndexing.AdvancedQueryParser.IntersectionParser"*

Choose {Debug/Release} depending on your current configuration.
  
## Launching:

1. Build all solution. No project references AdvancedQueryParser, so if you will not build it, you will not be able to use IntersectionParser (Yeah, I'm not good at naming).
2. Configure multiple startup projects: Solution -> Properties -> Common Properties -> Startup -> Choose "Multiple startup projects" -> Client: Start; Service: Start;
3. Run the solution
 
## Using:

### Service:
Service writes to console following events:
* Indexing of file
* Removing file from index

### Client:
#### Watch File/Directory
File/Directory buttons allows to choose file/directory and paster full path to the textbox. **Send** button checks whether file or directory path is provided and calls corresponding API method.

#### Get Files
You can write any string to query textbox and send request using **Send** button. If you are using **SimpleParser** then exact match for provided string will be returned. If you are using **IntersectionParser** then you can use queries like: *"Word1", "Word2"* and intersection of results for every word will be returned. (**SimpleIndexer** finds only words that have length > 2).

#### Log view
Every request writes result to log view in following format: {Time spent to get response}, {Response}, {Request}.

#### Host Url
You can change address of host using host url textbox.

#### Books library
For your convenience I have included several books in the repository. They reside in "{SolutionDir}BooksBackup" folder. After each build they are copied to folder "{SolutionDir}Books". So you can change that folder, remove books and after next build the folder will be restored. Only building will restore books folder and not just relaunching the solution.
