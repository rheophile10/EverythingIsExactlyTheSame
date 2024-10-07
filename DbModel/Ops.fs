namespace DBModel

module Ops
    
    open DBModel.MetaData
    open DBModel.Env
    
    open DBModel.Dbs.Postgres.DQL
    open DBModel.Dbs.SQLite.DQL
    open DBModel.Dbs.SQLServer.DQL
    
    let dql 
        (connString: string) 
        (query: string) 
        (parameters: (string * obj) list) 
        (dbType: DbModel.MetaData.Database)
        (outData: DbModel.MetaData.OutData)
        : DbModel.MetaData.OutData =
    
        match dbType with
        | Postgres ->
            let reader = DBModel.Dbs.Postgres.DQL.getReader connString query parameters
            match outData with
            | Reader -> reader
            | DataTable -> DBModel.Dbs.Postgres.DQL.convertReaderToDataTable reader
            | Collection -> DBModel.Dbs.Postgres.DQL.convertReaderToCollection reader
            | Json -> DBModel.Dbs.Postgres.DQL.convertReaderToJson reader
        | SQLite ->
            let reader = DBModel.Dbs.SQLite.DQL.getReader connString query parameters
            match outData with
            | Reader -> reader
            | DataTable -> DBModel.Dbs.SQLite.DQL.convertReaderToDataTable reader
            | Collection -> DBModel.Dbs.SQLite.DQL.convertReaderToCollection reader
            | Json -> DBModel.Dbs.SQLite.DQL.convertReaderToJson reader
        | SQLServer ->
            let reader = DBModel.Dbs.SQLServer.DQL.getReader connString query parameters
            match outData with
            | Reader -> reader
            | DataTable -> DBModel.Dbs.SQLServer.DQL.convertReaderToDataTable reader
            | Collection -> DBModel.Dbs.SQLServer.DQL.convertReaderToCollection reader
            | Json -> DBModel.Dbs.SQLServer.DQL.convertReaderToJson reader

    open DBModel.Dbs.Postgres.DML
    open DBModel.Dbs.SQLite.DML
    open DBModel.Dbs.SQLServer.DML

    let bulkLoad
        (connString: string) 
        (table: TableMetadata)
        (reader: IDataReader)
        (dbType: DbModel.MetaData.Database)
        =
    
        match dbType with
        | Postgres -> DBModel.Dbs.Postgres.DML.bulkLoad connString table reader
        | SQLite -> DBModel.Dbs.SQLite.DML.bulkLoad connString table reader
        | SQLServer -> DBModel.Dbs.SQLServer.DML.bulkLoad connString table reader

    let dml
        (connString: string) 
        (query: string) 
        (parameters: (string * obj) list) 
        (dbType: DbModel.MetaData.Database)
        =
    
        match dbType with
        | Postgres -> DBModel.Dbs.Postgres.DML.executeNonQuery connString query
        | SQLite -> DBModel.Dbs.SQLite.DML.executeNonQuery connString query
        | SQLServer -> DBModel.Dbs.SQLServer.DML.executeNonQuery connString query
        
    open DBModel.Dbs.Postgres.DDL
    open DBModel.Dbs.SQLite.DDL
    open DBModel.Dbs.SQLServer.DDL

    let ddl
        (dbMetadata: DbModel.MetaData.DatabaseMetadata)
        (dbType: DbModel.MetaData.Database)
        =
    
        match dbType with
        | Postgres -> DBModel.Dbs.Postgres.DDL.createTables dbMetadata
        | SQLite -> DBModel.Dbs.SQLite.DDL.createTables dbMetadata
        | SQLServer -> DBModel.Dbs.SQLServer.DDL.createTables dbMetadata
