namespace DbModel

module MetaData

    type Constraint =
        | PrimaryKey
        | ForeignKey of table: string * column: string
        | Unique

    type SqlColumnType =
        | Int of scale: int option
        | String of maxLength: int option
        | Bool
        | Decimal of precision: int * scale: int
        | Float
        | Double
        | DateTime
        | Date
        | Byte
        | ByteArray of maxLength: int option
        | Binary of maxLength: int option
        | Text

    type ColumnMetadata = {
        Name: string
        DataType: SqlColumnType
        Nullable: bool
        OrdinalPosition: int
        Length: int option
        DefaultValue: obj option
        Constraints: Constraint list
    }

    type TableMetadata = {
        TableName: string
        Columns: ColumnMetadata list
    }

    type Database = 
        | SQLServer
        | Postgres

    type DatabaseMetadata = {
        connectionStringEnvKey: string
        TypesToTables: Map<Type, TableMetadata>
        database: Database
    }

    open System.Text.Json
    open System.Data  
    open System.Collections.Generic

    type OutData =
        | Reader of IDataReader
        | DataTable of DataTable
        | Collection of List<Dictionary<string, obj>>
        | Json of string
        
