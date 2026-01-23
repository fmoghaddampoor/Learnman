---
description: Removes all previous publish_v* directories to clean the workspace.
---
1. Remove all directories matching 'publish_v*' in the root directory.

```powershell
Get-ChildItem -Path . -Directory -Filter "publish_v*" | Remove-Item -Recurse -Force
```
