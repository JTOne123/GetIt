namespace global

open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("GetIt.AssetGeneration")>]
[<assembly: InternalsVisibleTo("GetIt.Controller")>]
[<assembly: InternalsVisibleTo("GetIt.Test")>]
[<assembly: InternalsVisibleTo("GetIt.Windows")>]
[<assembly: InternalsVisibleTo("GetIt.UI")>]
do ()
