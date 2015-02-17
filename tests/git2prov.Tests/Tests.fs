module Tests
open ApprovalTests
open Xunit
open System.Diagnostics 
open Nessos.UnionArgParser
open Main
open System.IO

type Approve () = class end
    with static member string (s:string) =
      Approvals.Verify s

let clone repo =
  let dir = sprintf "Examples/%s" repo        
  if Directory.Exists dir then Directory.Delete (dir,true)
  
  let ps = Process.Start ("git",sprintf "clone ../tests/git2prov.Tests/Examples/%s" repo)
  ps.WaitForExit()
  repo


let g2p args =
  let parser = UnionArgParser.Create<Arguments>()
  let args = parser.PrintCommandLine args |> String.concat "  " 

  let psi = ProcessStartInfo (FileName="git2prov.exe",
                              UseShellExecute=false,
                              Arguments=args,
                              RedirectStandardOutput=true)
  let ps = Process.Start psi
  let os = ps.StandardOutput.ReadToEnd()
  ps.WaitForExit(100) |> ignore
  os
 
  
[<Fact>]
let ``Show changes from HEAD`` () =
  clone "testrepo.git"
  g2p [Main.Path "testrepo"
       ShowHistory
       Since "HEAD~1"]
  |> Approve.string
  
 
