namespace Metadata

module Query =

    open Metadata.Metadata

    type JoinType =
        | InnerJoin
        | LeftJoin
        | RightJoin
        | FullJoin
        | CrossJoin

    type OrderDirection = 
        | Asc
        | Desc

    type UnionType =
        | Union
        | UnionAll
        | Intersect
        | Except

    type SqlOperator =
        | Equal
        | NotEqual
        | GreaterThan
        | LessThan
        | GreaterThanOrEqual
        | LessThanOrEqual
        | Like
        | NotLike
        | ILike
        | In
        | NotIn
        | Between
        | NotBetween
        | IsNull
        | IsNotNull
        | And
        | Or

    and SqlValue =
        | IntValue of int
        | Int64Value of int64
        | FloatValue of float
        | DoubleValue of double
        | DecimalValue of decimal
        | StringValue of string
        | BoolValue of bool
        | DateTimeValue of System.DateTime
        | DateValue of System.DateTime
        | ByteValue of byte
        | ByteArrayValue of byte[]
        | GuidValue of System.Guid
        | Null
        | ListValue of SqlValue list
        | SubQueryValue of SqlQuery

    and ColumnReference = {
        TableName: string option
        TableAlias: string option
        ColumnName: string
        ColumnAlias: string option
        Metadata: ColumnMetadata option
    }

    and Condition =
        | Comparison of left: ColumnReference * op: SqlOperator * right: SqlValue
        | ColumnComparison of left: ColumnReference * op: SqlOperator * right: ColumnReference
        | NestedCondition of Condition
        | CombinedCondition of left: Condition * op: SqlOperator * right: Condition

    and Join = {
        JoinType: JoinType
        RightSource: QuerySource
        OnConditions: Condition list
    }

    and QuerySource =
        | Table of tableName: string * alias: string option
        | SubQuery of query: SqlQuery * alias: string
        | Join of Join

    and Aggregation =
        | Count of ColumnReference option
        | Sum of ColumnReference
        | Avg of ColumnReference
        | Min of ColumnReference
        | Max of ColumnReference
        | CustomAggregation of functionName: string * ColumnReference

    and SelectElement =
        | Column of ColumnReference
        | Aggregation of Aggregation * alias: string option
        | Expression of expression: string * alias: string option
        | SubQueryExpression of query: SqlQuery * alias: string

    and CTE = {
        Name: string
        Query: SqlQuery
    }

    and SqlQuery = {
        CTEs: CTE list
        Select: SelectElement list
        From: QuerySource option
        Where: Condition option
        GroupBy: ColumnReference list
        Having: Condition option
        OrderBy: (ColumnReference * OrderDirection) list
        Limit: int option
        Offset: int option
        Unions: (UnionType * SqlQuery) list
    }
