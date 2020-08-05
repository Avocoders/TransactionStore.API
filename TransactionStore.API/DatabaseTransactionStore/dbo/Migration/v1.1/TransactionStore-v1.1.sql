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
alter procedure [dbo].[Transaction_Add]
	@accountId bigint,
	@typeId tinyint,
	@currencyId tinyint,
	@amount money	
as
begin
	declare @timestamp datetime2 = sysdatetime()
	insert into [dbo].[Transaction]
		   (AccountId, 
		    TypeId,
			CurrencyId, 
			Amount, 
			[Timestamp]) 
	values		
		   (@accountId, 
			@typeId, 
			@currencyId, 
			@amount, 
			@timestamp)
	select scope_identity();
end
go
alter procedure Transaction_AddTransfer
	@accountId bigint,
	@typeId tinyint,
	@currencyId tinyint,
	@amount money,
	@accountIdReceiver bigint
as
begin
	declare @timestamp datetime2 = sysdatetime()
	insert into [dbo].[Transaction] 
		   (AccountId, 
			TypeId, 
			CurrencyId, 
			Amount, 
			[Timestamp])
	values 
		   (@accountId, 
			@typeId,
			@currencyId, 
			-@amount, 
			@timestamp)
	declare @account bigint set @account = scope_identity()
	insert into [dbo].[Transaction] 
		   (AccountId, 
			TypeId, 
			CurrencyId, 
			Amount, 
			[Timestamp])
	values 
		   (@accountIdReceiver, 
			@typeId, 
			@currencyId,
			@amount, 
			@timestamp)
	select @account as [account]
	union select scope_identity()
end
go
alter procedure [dbo].[Transaction_GetById]
	@Id bigint
as
begin
    select 
       id,
       AccountId,
       Amount,
       [Timestamp],
       typeId,
       currencyId
     into #SearchResult 
     from dbo.[Transaction] 
     where (Id=@id)
    
     select 
         t.Id,
         t.AccountId,
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
           AccountId,
           Amount,
           [Timestamp],
           typeId,
           currencyId
       from #SearchResult 
end
go
create procedure [dbo].[Transaction_GetByAccountId]
	@accountId bigint
as
begin
    select 
       id,
       AccountId,
       Amount,
       [Timestamp],
       typeId,
       currencyId
     into #SearchResult 
     from dbo.[Transaction] 
     where (AccountId=@accountId )
    
     select 
         t.Id,
         t.AccountId,
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
           AccountId,
           Amount,
           [Timestamp],
           typeId,
           currencyId
       from #SearchResult 
end
go
alter procedure CreateStrings
	@rowValue bigint 
as
	begin
		declare @length int = 0
		while @length < @rowValue
		begin
		declare @accountId_1 bigint
        declare @accountId_2 bigint
		declare @typeId tinyint
		declare @currencyId tinyint
		declare @amountDeposit money
		declare @timestamp datetime2
		declare @amountWithdraw money

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

		if @typeId=1
			begin
				insert into [Transaction](
					AccountId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp])
					values (@accountId_1,
							@typeId,
							@currencyId,
							@amountDeposit,
							@timestamp)
		end

		if @typeId=2
			begin
				insert into [Transaction](
					AccountId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp])
					values (@accountId_1,
							@typeId,
							@currencyId,
							@amountWithdraw,
							@timestamp)
		end

		if @typeId=3
			begin
				insert into [Transaction](
					AccountId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp])
					values (@accountId_1,
							@typeId,
							@currencyId,
							@amountDeposit,
							@timestamp)
				insert into [Transaction](
					AccountId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp])
					values (@accountId_2,
							@typeId,
							@currencyId,
							@amountWithdraw,
							@timestamp)
		end
	set @length = @length+1
	end
end
go
alter procedure  Transaction_Search
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
            join [Transaction] t on s.CurrencyId=t.CurrencyId and 
                                    s.TypeId = t.TypeId and
                                    s.[Timestamp]=t.[Timestamp] and
                                    s.Amount<>t.Amount and
                                    abs(s.Amount)=abs(t.Amount)
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