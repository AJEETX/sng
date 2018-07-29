Sub OnStartup
   Set o = CreateObject("Ripp.Sng.MediaMonkey.Akka.Agent.MediaMonkeyAkkaProxy")
   o.Init(SDB)
End Sub