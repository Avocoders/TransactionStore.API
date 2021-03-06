﻿DECLARE @currentDBVersion nvarchar(8)
set @currentDBVersion = (select top(1) DbVersion from [dbo].[DbVersion] order by Created desc)
go

IF @currentDBVersion >='1.1'
set noexec on

truncate table dbo.[Transaction]
go
alter table dbo.[Transaction]
drop column [LeadId]
go
alter table dbo.[Transaction]
add [AccountId] bigint not null
go
alter table dbo.[Transaction]
add [ExchangeRates] decimal(18,10) not null
go
create table [dbo].[CurrencyRates](
	[id] [tinyint] NOT NULL,
	[Code] [nvarchar](3) NULL,
	[Rate] [decimal](18, 10) NULL
) on [PRIMARY]

go
CREATE PROCEDURE [dbo].[CurrencyRates_Update] 
@code nvarchar(3),
@rate decimal(18,10)
AS
BEGIN
	update dbo.[CurrencyRates]
	set Rate=@rate
	where Code = @code
END
go
CREATE PROCEDURE dbo.CurrencyRates_GetById 
@id tinyint
AS
BEGIN
	select c.Rate
	from dbo.CurrencyRates as c
	where Id = @id
END
GO
CREATE PROCEDURE [dbo].[Transaction_DeleteAll]
AS
BEGIN
	truncate table [Transaction]
END
GO
ALTER procedure [dbo].[Transaction_Add]
	@accountId bigint,
	@typeId tinyint,
	@currencyId tinyint,
	@amount money,
	@exchangeRates decimal(18,10),
	@timestamp nvarchar(40)
as
begin
	if @timestamp is not null
	Begin
	    declare @newTimestamp datetime2(7)
		set @newTimestamp = cast (@timestamp as datetime2)
	end
	declare @lastTimestamp datetime2(7) 
	set @lastTimestamp = (select max(t.[Timestamp]) From [Transaction] t where t.AccountId = @accountId)	
	if (@typeId =1 or @timestamp is null or @newTimestamp = @lastTimestamp  )
		begin
			insert into [dbo].[Transaction]
				   (AccountId, 
					TypeId,
					CurrencyId, 
					Amount, 
					ExchangeRates,
					[Timestamp]) 
			values		
				   (@accountId, 
					@typeId, 
					@currencyId, 
					@amount, 
					@exchangeRates,
					sysdatetime())
			select CAST(SCOPE_IDENTITY() as [bigint]);
		end
	else 
		RAISERROR (50001,-1,16);
end
go
ALTER procedure [dbo].[Transaction_AddTransfer]
	@accountId bigint,
	@typeId tinyint,
	@currencyId tinyint,
	@amount1 money,
	@amount2 money,
	@accountIdReceiver bigint,
	@receiverCurrencyId tinyint,
	@exchangeRates1 decimal (18,10), 
	@exchangeRates2 decimal (18,10),
	@timestamp nvarchar(40)
	
as
begin
	if @timestamp is not null
		Begin
			declare @newTimestamp datetime2(7)
			set @newTimestamp = cast (@timestamp as datetime2)
	    end
	declare @lastTimestamp datetime2(7) 
	set @lastTimestamp = (select max(t.[Timestamp]) From [Transaction] t where t.AccountId = @accountId)	
	if (@timestamp is null or @newTimestamp = @lastTimestamp  )
		begin
			insert into [dbo].[Transaction] 
				   (AccountId, 
					TypeId, 
					CurrencyId, 
					Amount, 
					ExchangeRates,
					[Timestamp])
			values 
				   (@accountId, 
					@typeId,
					@currencyId, 
					-@amount1,
					@exchangeRates1,
					sysdatetime())
			declare @account bigint set @account = scope_identity()
			insert into [dbo].[Transaction] 
				   (AccountId, 
					TypeId, 
					CurrencyId, 
					Amount, 
					ExchangeRates,
					[Timestamp])
			values 
				   (@accountIdReceiver, 
					@typeId, 
					@receiverCurrencyId,
					@amount2,
					@exchangeRates2,
					sysdatetime())
			select @account as [account]
			union select scope_identity()
		end
	else 
		RAISERROR (50001,-1,16);
end

go
ALTER procedure [dbo].[Transaction_GetById]
	@Id bigint
as
begin
    select 
       id,
       AccountId,
       Amount,
       [Timestamp],
       typeId
     into #SearchResult 
     from dbo.[Transaction] 
     where (Id=@id)
    
     select 
         t.Id,
         t.AccountId,
         t.Amount,
         t.[Timestamp],
         t.typeId as id         
      from #SearchResult s
      join [Transaction] t on  
                         s.TypeId = t.TypeId and
                         s.[Timestamp]=t.[Timestamp] 
                                               
      union select 
           Id,
           AccountId,
           Amount,
           [Timestamp],
           typeId
       from #SearchResult 
end
go
Create procedure [dbo].[Transaction_GetByAccountId]
	@accountId bigint
as
begin
    select 
       id,
       AccountId,
       Amount,
       [Timestamp],
       typeId
     into #SearchResult 
     from dbo.[Transaction] 
     where (AccountId=@accountId )
    
     select 
         t.Id,
         t.AccountId,
         t.Amount,
         t.[Timestamp],
         t.typeId as id
      from #SearchResult s
      join [Transaction] t on s.TypeId = t.TypeId and
                              s.[Timestamp]=t.[Timestamp] 
                         
      union select 
           Id,
           AccountId,
           Amount,
           [Timestamp],
           typeId
       from #SearchResult 
end
go
ALTER procedure [dbo].[CreateStrings]
	@rowValue bigint 
as
	begin
		declare @length int = 0
		declare @accountId_1 bigint
		declare @accountId_2 bigint
		declare @typeId tinyint
		declare @currencyId tinyint
		declare @amountDeposit money
		declare @timestamp datetime2
		declare @amountWithdraw money
		declare @exchangeRates decimal(18,10)

		create table #RandomExchangeRates
			(id int,
			 [Rate] decimal(18,10))
		

	    insert into #RandomExchangeRates
			(id, [Rate])
			select 1, 86.01 union
			select 2, 1.17 union
			select 3, 1 union
			select 4, 124.60 

  	while @length < @rowValue
		begin			

		set @accountId_1=(select round((rand()*100000),1))

		set @accountId_2=@accountId_1+1

		set @typeId =(select top 1 Id
					from [Type]
					order by newid())

		set @currencyId =(select top 1 Id
					from [Currency]
					order by newid())

		set @amountDeposit = (select round((rand()*100000),1))

		set @amountWithdraw=@amountDeposit*(-1)

		set @timestamp= dateadd(day, rand()*(3000-1)+1, '2010-01-01')

		set @exchangeRates =(select top 1 Rate
							from #RandomExchangeRates
							where id = @currencyId
							order by newid())

		if @typeId=1
			begin
				insert into [Transaction](
					AccountId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp],
					ExchangeRates)
					values (@accountId_1,
							@typeId,
							@currencyId,
							@amountDeposit,
							@timestamp,
							@exchangeRates)
		end

		if @typeId=2
			begin
				insert into [Transaction](
					AccountId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp],
					ExchangeRates)
					values (@accountId_1,
							@typeId,
							@currencyId,
							@amountWithdraw,
							@timestamp,
							@exchangeRates)
		end

		if @typeId=3
			begin
				insert into [Transaction](
					AccountId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp],
					ExchangeRates)
					values (@accountId_1,
							@typeId,
							@currencyId,
							@amountDeposit,
							@timestamp,
							@exchangeRates)
				insert into [Transaction](
					AccountId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp],
					ExchangeRates)
					values (@accountId_2,
							@typeId,
							@currencyId,
							@amountWithdraw,
							@timestamp,
							@exchangeRates)
		end
	set @length = @length+1
	end
	drop table #RandomExchangeRates
end
go
create Procedure [dbo].[Transaction_GetBalanceByAccountId]
@accountId bigint
as
Begin
	select SUM(t.Amount) as Balance, max(t.[Timestamp]) as [Timestamp] From [Transaction] t
	Where t.AccountId=@accountId
	Group By t.AccountId
end
go
ALTER procedure  [dbo].[Transaction_Search]
	@accountId bigint = null,
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
    if @accountId >0
      begin
				set @partresult= @partresult + ' and t.AccountId = ' + convert(nvarchar(20), @accountId)
	
	  set @resultSql =
                'select 
                t.id,
                t.AccountId,
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
                t.AccountId,
                t.Amount,
                t.[Timestamp],
                t.typeId as id,
                t.currencyId as id
            from #SearchResult s
            join [Transaction] t on  
                                    s.TypeId = t.TypeId and
                                    s.[Timestamp]=t.[Timestamp] 
                                    
            union select 
                Id,
                AccountId,
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
                t.id,
                t.AccountId,
                t.Amount,
                t.[Timestamp],
                t.typeId as id,
                t.currencyId as id
            from dbo.[Transaction]  as t
            where 1=1 ' 
			+ @partresult
  
        end
		exec  sp_sqlexec @resultSql
end
go
INSERT INTO dbo.[DbVersion] (Created, DbVersion) VALUES (SYSDATETIME(), '1.1')

go
set noexec off