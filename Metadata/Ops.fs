namespace Metadata

module Ops =
    
    open Metadata.MetaData
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
    open Metadata.Dbs.SQLite.DQL
    open Metadata.Dbs.SQLServer.DQL
    
    let dql 
        (connString: string) 
        (query: string) 
        (parameters: (string * obj) list) 
        (dbType: Metadata.MetaData.Database)
        (outData: Metadata.MetaData.OutData)
        : Metadata.MetaData.OutData =
    
        match dbType with
        | Postgres ->
            let reader = Metadata.Dbs.Postgres.DQL.getReader connString query parameters
            match outData with
            | Reader -> reader
            | DataTable -> Metadata.Dbs.Postgres.DQL.convertReaderToDataTable reader
            | Collection -> Metadata.Dbs.Postgres.DQL.convertReaderToCollection reader
            | Json -> Metadata.Dbs.Postgres.DQL.convertReaderToJson reader
        | SQLite ->
            let reader = Metadata.Dbs.SQLite.DQL.getReader connString query parameters
            match outData with
            | Reader -> reader
            | DataTable -> Metadata.Dbs.SQLite.DQL.convertReaderToDataTable reader
            | Collection -> Metadata.Dbs.SQLite.DQL.convertReaderToCollection reader
            | Json -> Metadata.Dbs.SQLite.DQL.convertReaderToJson reader
        | SQLServer ->
            let reader = Metadata.Dbs.SQLServer.DQL.getReader connString query parameters
            match outData with
            | Reader -> reader
            | DataTable -> Metadata.Dbs.SQLServer.DQL.convertReaderToDataTable reader
            | Collection -> Metadata.Dbs.SQLServer.DQL.convertReaderToCollection reader
            | Json -> Metadata.Dbs.SQLServer.DQL.convertReaderToJson reader

    open Metadata.Dbs.Postgres.DML
    open Metadata.Dbs.SQLite.DML
    open Metadata.Dbs.SQLServer.DML

    let bulkLoad
        (connString: string) 
        (table: TableMetadata)
        (reader: IDataReader)
        (dbType: Metadata.MetaData.Database)
        =
    
        match dbType with
        | Postgres -> Metadata.Dbs.Postgres.DML.bulkLoad connString table reader
        | SQLite -> Metadata.Dbs.SQLite.DML.bulkLoad connString table reader
        | SQLServer -> Metadata.Dbs.SQLServer.DML.bulkLoad connString table reader

    let dml
        (connString: string) 
        (query: string) 
        (parameters: (string * obj) list) 
        (dbType: Metadata.MetaData.Database)
        =
    
        match dbType with
        | Postgres -> Metadata.Dbs.Postgres.DML.executeNonQuery connString query
        | SQLite -> Metadata.Dbs.SQLite.DML.executeNonQuery connString query
        | SQLServer -> Metadata.Dbs.SQLServer.DML.executeNonQuery connString query
        
    open Metadata.Dbs.Postgres.DDL
    open Metadata.Dbs.SQLite.DDL
    open Metadata.Dbs.SQLServer.DDL

    let ddl
        (dbMetadata: Metadata.MetaData.Metadata)
        (dbType: Metadata.MetaData.Database)
        =
    
        match dbType with
        | Postgres -> Metadata.Dbs.Postgres.DDL.createTables dbMetadata
        | SQLite -> Metadata.Dbs.SQLite.DDL.createTables dbMetadata
        | SQLServer -> Metadata.Dbs.SQLServer.DDL.createTables dbMetadata
