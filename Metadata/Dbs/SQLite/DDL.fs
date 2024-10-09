namespace Metadata.Dbs.Sqlite

module DDL =

    open Metadata.Metadata
    open Metadata.Env

    let private sqlColumnTypeToSqliteType (colType: SqlColumnType) =
        match colType with
        | Int _ -> "INTEGER"
        | String (Some maxLength) -> sprintf "VARCHAR(%d)" maxLength
        | String None -> "TEXT"
        | Bool -> "BOOLEAN"
        | Decimal (precision, scale) -> sprintf "DECIMAL(%d, %d)" precision scale
        | Float -> "REAL"
        | Double -> "DOUBLE"
        | DateTime -> "DATETIME"
        | Date -> "DATE"
        | Byte -> "TINYINT"
        | ByteArray _ -> "BLOB"
        | Binary _ -> "BLOB"
        | Text -> "TEXT"

    let private generateCreateTableScript (table: TableMetadata) =
        let columnsSql =
            table.Columns
            |> List.map (fun col ->
                let dataType = sqlColumnTypeToSqliteType col.DataType
                let nullable = if col.Nullable then "" else "NOT NULL"
                let constraints =
                    col.Constraints
                    |> List.map (function
                        | PrimaryKey -> "PRIMARY KEY"
                        | Unique -> "UNIQUE"
                        | ForeignKey (refTable, refColumn) ->
                            sprintf "REFERENCES %s(%s)" refTable refColumn
                    )
                    |> String.concat " "
                sprintf "%s %s %s %s" col.Name dataType nullable constraints
            )
            |> String.concat ",\n"

        sprintf "CREATE TABLE IF NOT EXISTS %s (\n%s\n);" table.TableName columnsSql

    open Microsoft.Data.Sqlite

    let createTables (dbMetadata: Metadata) =
        let connString = getConnectionString dbMetadata.connectionStringEnvKey
        use conn = new SqliteConnection(connString)
        conn.Open()

        match dbMetadata.TypesToTables with
        | Some typesToTables ->
            typesToTables
            |> Map.iter (fun _ table ->
                let script = generateCreateTableScript table
                use cmd = new SqliteCommand(script, conn)
                cmd.ExecuteNonQuery() |> ignore
            )
        | None -> printfn "No table metadata available."
        
        conn.Close()
