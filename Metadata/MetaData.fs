namespace Metadata

module Metadata =

    type Constraint =
        | PrimaryKey
        | ForeignKey of table: string * column: string
        | Unique

    type SqlType =
        | Int of scale: int option
        | Int64 of scale: int option
        | Float
        | Double
        | Decimal of precision: int * scale: int
        | String of maxLength: int option
        | Text
        | Bool
        | DateTime
        | Date
        | Byte
        | ByteArray of maxLength: int option
        | Binary of maxLength: int option
        | Guid

    type ColumnMetadata = {
        Name: string
        DataType: SqlType
        Nullable: bool
        OrdinalPosition: int
        Length: int option
        DefaultValue: SqlValue option
        Constraints: Constraint list
    }

    let createColumn name dataType nullable ordinalPosition constraints =
        { 
            Name = name
            DataType = dataType
            Nullable = nullable
            OrdinalPosition = ordinalPosition
            Length = None
            DefaultValue = None
            Constraints = constraints 
        }

    type TableMetadata = {
        TableName: string
        Columns: ColumnMetadata list
    }

    type Database = 
        | SqlServer
        | Postgres
        | Sqlite

    type Metadata = {
        connectionStringEnvKey: string        
        TypesToTables: Map<string, TableMetadata> option
        database: Database
    }

    open System.IO
    open FSharp.Json

    let writeToJson<'T> (data: 'T) (filePath: string) =
        data
        |> Json.serialize
        |> fun json -> File.WriteAllText(filePath, json)

    let loadFromJson<'T> (filePath: string) : 'T =
        filePath
        |> File.ReadAllText
        |> Json.deserialize<'T>

