create procedure  Transaction_Search
@leadId bigint = null,
@typeId int = null,
@currencyId tinyint = null,
@amountBegin money = null,
@amountEnd money = null,
@fromDate datetime2(7) = null,
@tillDate datetime2(7) = null
as
begin
declare @resultSql nvarchar(max)
declare @partresult nvarchar(max) = ''

if @typeId>0
		begin
			set @partresult= @partresult + ' and t.typeId = ' + convert(nvarchar(1), @typeId)
		end

if @currencyId>0
		begin
			set @partresult= @partresult + ' and t.currencyId = ' + convert(nvarchar(1), @currencyId)
		end

if @amountBegin is not null
		begin
			set @partresult = @partresult + ' and t.Amount >=''' + convert(nvarchar(10), @amountBegin)+''''
		end

if @amountEnd is not null
		begin
			set @partresult = @partresult + ' and t.Amount <=''' + convert(nvarchar(10), @amountEnd)+''''
		end

if @fromDate is not null
		begin
			set @partresult = @partresult + ' and t.[Timestamp] >=''' + convert(nvarchar(10), @fromDate)+''''
		end

if @tillDate is not null
		begin
			set @partresult = @partresult + ' and t.[Timestamp] <=''' + convert(nvarchar(10), @tillDate)+''''
		end
 if @leadId >0
      begin
				set @partresult= @partresult + ' and t.leadId = ' + convert(nvarchar(20), @leadId)
	
	  set @resultSql =
                'select 
                t.id,
                t.LeadId,
                t.Amount,
                t.[Timestamp],
                t.typeId,
                t.currencyId
            into #SearchResult 
            from dbo.[Transaction] as t
            where 1=1' 

 + @partresult
           
            +' select 
                t.Id,
                t.LeadId,
                t.Amount,
                t.[Timestamp],
                t.typeId as id,
                t.currencyId as id
            from #SearchResult s
            join [Transaction] t on s.CurrencyId=t.CurrencyId and 
                                    s.TypeId = t.TypeId and
                                    s.[Timestamp]=t.[Timestamp] and
                                    s.Amount<>t.Amount and
                                    abs(s.Amount)=abs(t.Amount)
            union select 
                Id,
                LeadId,
                Amount,
                [Timestamp],
                typeId,
                currencyId
            from #SearchResult'
        end
    else
        begin
		set @resultSql = 
		'select 
                t.id,                t.LeadId,                t.Amount,                t.[Timestamp],                t.typeId as id,                t.currencyId as id            from dbo.[Transaction]  as t
            where 1=1 ' 
			+ @partresult
  
        end
		exec  sp_sqlexec @resultSql
end