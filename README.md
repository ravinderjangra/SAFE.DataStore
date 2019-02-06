# Safe-datastore-getting-started-dotnet

This repository contains source code for the SAFE.DataStore library (netcore 3.0, C# 8.0).

|SAFE.DataStore |
|:-:|:-:|
|[![Build Status](https://dev.azure.com/oetyng/datastore/build/status/SAFE-DataStore-CI)](https://dev.azure.com/oetyng/datastore/_build/latest?definitionId=6)|

## DataStore (netcore 3.0)

### Features 

Includes:
 - Classes to build a database for the SAFE Network
 - Modes of Authentication
     - Mock authentication using the mock Authenticator API
     - Mock authentication using the [mock-safe-browser](https://github.com/maidsafe/safe_browser/releases/latest)
     - Test net authentication using the [safe-browser](https://github.com/maidsafe/safe_browser/releases/latest)
 - Client
	 - Create and connect to a db
     - Managing db content
	 - In memory db
	 - Mock network db

Not yet included:
 - Connecting to live network.

### Prerequisites

- Visual Studio 2019 with dotnet core development workload

### Supported Platforms

- Windows (x64)

### Required SDK/Tools
- netcore 3.0 SDK

## Further Help

Get your developer related questions clarified on the [SAFE Dev Forum](https://forum.safedev.org/). If you're looking to share any other ideas or thoughts on the SAFE Network you can reach out on the [SAFE Network Forum](https://safenetforum.org/).


## Contribution

Copyrights in the SAFE Network are retained by their contributors. No copyright assignment is required to contribute to this project.


## License

This SAFE Network library is dual-licensed under the Modified BSD ([LICENSE-BSD](LICENSE-BSD) https://opensource.org/licenses/BSD-3-Clause) or the MIT license ([LICENSE-MIT](LICENSE-MIT) https://opensource.org/licenses/MIT) at your option.
