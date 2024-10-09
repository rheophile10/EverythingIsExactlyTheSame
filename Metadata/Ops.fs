namespace Metadata

module Ops =
    
    open Metadata.Metadata
    open Metadata.Env
    
    open System.Text.Json
    open System.Data  
    open System.Collections.Generic

    type OutData =
        | Reader of IDataReader
        | DataTable of DataTable
        | Collection of List<Dictionary<string, obj>>
        | Json of string
    
    open Metadata.Dbs.Postgres.DQL
    open Metadata.Dbs.Sqlite.DQL
    open Metadata.Dbs.SqlServer.DQL
    
    let dql 
        (connString: string) 
        (query: string) 
        (parameters: (string * obj) list) 
        (dbType: Metadata.Metadata.Database)
        (outData: OutData)
        : OutData =
        
        match dbType with
        | Postgres ->
            let reader = Metadata.Dbs.Postgres.DQL.getReader connString query parameters :?> Npgsql.NpgsqlDataReader
            match outData with
            | Reader _ -> Reader reader
            | DataTable _ -> DataTable (Metadata.Dbs.Postgres.DQL.convertReaderToDataTable reader)
            | Collection _ -> Collection (Metadata.Dbs.Postgres.DQL.convertReaderToCollection reader)
            | Json _ -> Json (Metadata.Dbs.Postgres.DQL.convertReaderToJson reader)
        | Sqlite ->
            let reader = Metadata.Dbs.Sqlite.DQL.getReader connString query parameters 
            match outData with
            | Reader _ -> Reader reader
            | DataTable _ -> DataTable (Metadata.Dbs.Sqlite.DQL.convertReaderToDataTable reader)
            | Collection _ -> Collection (Metadata.Dbs.Sqlite.DQL.convertReaderToCollection reader)
            | Json _ -> Json (Metadata.Dbs.Sqlite.DQL.convertReaderToJson reader)
        | SqlServer ->
            let reader = Metadata.Dbs.SqlServer.DQL.getReader connString query parameters
            match outData with
            | Reader _ -> Reader reader
            | DataTable _ -> DataTable (Metadata.Dbs.SqlServer.DQL.convertReaderToDataTable reader)
            | Collection _ -> Collection (Metadata.Dbs.SqlServer.DQL.convertReaderToCollection reader)
            | Json _ -> Json (Metadata.Dbs.SqlServer.DQL.convertReaderToJson reader)

    open Metadata.Dbs.Postgres.DML
    open Metadata.Dbs.Sqlite.DML
    open Metadata.Dbs.SqlServer.DML

    let bulkLoad

        (connString: string) 
        (table: TableMetadata)
        (reader: IDataReader)
        (dbType: Metadata.Metadata.Database)
        =
    
        match dbType with
        | Postgres -> Metadata.Dbs.Postgres.DML.bulkLoad connString table reader
        | Sqlite -> Metadata.Dbs.Sqlite.DML.bulkLoad connString table reader
        | SqlServer -> Metadata.Dbs.SqlServer.DML.bulkLoad connString table reader

    let dml
        (connString: string) 
        (query: string) 
        (parameters: (string * obj) list) 
        (dbType: Metadata.Metadata.Database)
        =
    
        match dbType with
        | Postgres -> Metadata.Dbs.Postgres.DML.executeNonQuery connString query
        | Sqlite -> Metadata.Dbs.Sqlite.DML.executeNonQuery connString query
        | SqlServer -> Metadata.Dbs.SqlServer.DML.executeNonQuery connString query
        
    open Metadata.Dbs.Postgres.DDL
    open Metadata.Dbs.Sqlite.DDL
    open Metadata.Dbs.SqlServer.DDL

    let ddl
        (dbMetadata: Metadata.Metadata.Metadata)
        (dbType: Metadata.Metadata.Database)
        =
    
        match dbType with
        | Postgres -> Metadata.Dbs.Postgres.DDL.createTables dbMetadata
        | Sqlite -> Metadata.Dbs.Sqlite.DDL.createTables dbMetadata
        | SqlServer -> Metadata.Dbs.SqlServer.DDL.createTables dbMetadata