namespace Metadata.Dbs.SQLite

module DML =

    open System.Data
    open Microsoft.Data.SQLite
    open Metadata.Metadata
    open System.Text
    open System

    let bulkLoadData
        (connString: string)
        (table: TableMetadata)
        (reader: IDataReader) =

        use conn = new SQLiteConnection(connString)
        conn.Open()

        use transaction = conn.BeginTransaction()

        let columns = table.Columns |> List.map (fun col -> col.Name) |> String.concat ", "
        let placeholders = table.Columns |> List.map (fun _ -> "?") |> String.concat ", "

        let insertCommandText = sprintf "INSERT INTO %s (%s) VALUES (%s)" table.TableName columns placeholders

        use cmd = new SQLiteCommand(insertCommandText, conn, transaction)

        for col in table.Columns do
            cmd.Parameters.Add(new SQLiteParameter()) |> ignore

        while reader.Read() do
            for i in 0 .. table.Columns.Length - 1 do
                let columnName = table.Columns.[i].Name
                let value = reader.[columnName]
                cmd.Parameters.[i].Value <- if isNull value then box DBNull.Value else value

            cmd.ExecuteNonQuery() |> ignore

        transaction.Commit()

        reader.Close()

    let executeNonQuery (connString: string) (query: string) =
        use conn = new SQLiteConnection(connString)
        conn.Open()

        use cmd = new SQLiteCommand(query, conn)
        cmd.ExecuteNonQuery() |> ignore
        conn.Close()