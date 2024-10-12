namespace Metadata

module Ops =
    
    open Metadata.Metadata
    open Metadata.Env
    
    open FSharp.Json
    open System.Data  

    let convertReaderToDataTable (reader: IDataReader) : DataTable =
        let dataTable = new DataTable()
        dataTable.Load(reader)
        reader.Close()
        dataTable

    let convertReaderToSequence (reader: IDataReader) : seq<Map<string, obj>> =
        Seq.initInfinite (fun _ -> reader.Read())
        |> Seq.takeWhile id
        |> Seq.map (fun _ ->
            Seq.init reader.FieldCount (fun i -> 
                let value = if reader.IsDBNull(i) then null else reader.GetValue(i)
                reader.GetName(i), value
            )
            |> Map.ofSeq  
        )
        |> fun sequenceOfRows -> 
            reader.Close()  
            sequenceOfRows

    let convertReaderToJson (reader: IDataReader) : string =
        reader
        |> convertReaderToSequence
        |> Json.serialize
    
    open Npgsql
    open Microsoft.Data.SqlClient
    open Microsoft.Data.Sqlite

    let addParameter (cmd: IDbCommand) (name: string, value: obj) =
        match cmd with
        | :? NpgsqlCommand as npgsqlCmd -> npgsqlCmd.Parameters.AddWithValue(name, value) |> ignore
        | :? SqliteCommand as sqliteCmd -> sqliteCmd.Parameters.AddWithValue(name, value) |> ignore
        | :? SqlCommand as sqlCmd -> sqlCmd.Parameters.AddWithValue(name, value) |> ignore
        | _ -> failwith "Unsupported command type"

    let createConnectionAndCommand 
        (dbType: Database) 
        (connString: string) 
        (query: string) 
        : IDbConnection * IDbCommand =
        match dbType with
        | Postgres ->
            let conn = new NpgsqlConnection(connString) :> IDbConnection
            let cmd = new NpgsqlCommand(query) :> IDbCommand
            conn, cmd
        | Sqlite ->
            let conn = new SqliteConnection(connString) :> IDbConnection
            let cmd = new SqliteCommand(query) :> IDbCommand
            conn, cmd
        | SqlServer ->
            let conn = new SqlConnection(connString) :> IDbConnection
            let cmd = new SqlCommand(query) :> IDbCommand
            conn, cmd

    let getReader 
        (dbType: Database) 
        (connString: string) 
        (query: string) 
        (parameters: (string * obj) list) 
        : IDataReader =
        let conn, cmd = createConnectionAndCommand dbType connString query
        conn.Open()
        cmd.Connection <- conn
        parameters |> List.iter (fun (name, value) -> addParameter cmd (name, value))
        cmd.ExecuteReader()

    type OutData =
        | Reader of IDataReader
        | DataTable of DataTable
        | Sequence of seq<Map<string, obj>>
        | Json of string

    let dql 
        (connString: string) 
        (query: string) 
        (parameters: (string * obj) list) 
        (dbType: Database)
        (outData: OutData)
        : OutData =
        
        let readerFunc = getReader dbType connString query parameters

        match outData with
        | Reader _ -> Reader readerFunc
        | DataTable _ -> 
            readerFunc 
            |> convertReaderToDataTable
            |> DataTable
        | Sequence _ -> 
            readerFunc 
            |> convertReaderToSequence
            |> Sequence
        | Json _ -> 
            readerFunc 
            |> convertReaderToJson
            |> Json

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