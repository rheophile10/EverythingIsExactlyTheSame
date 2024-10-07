namespace DbModel

module Env
    
    open DotNetEnv

    let getConnectionString (connectionStringEnvKey:string) =
        DotNetEnv.Env.Load()
        let connString = DotNetEnv.Env.GetString(connectionStringEnvKey)
        connString