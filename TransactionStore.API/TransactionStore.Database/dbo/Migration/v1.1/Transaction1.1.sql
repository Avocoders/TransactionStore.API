--alter table [Transaction]
--drop column [LeadId]

--alter table [Transaction]
--add [AccountId] bigint not null

--alter procedure [dbo].[Transaction_Add]
--	@accountId bigint,
--	@typeId tinyint,
--	@currencyId tinyint,
--	@amount money	
--as
--begin
--	declare @timestamp datetime2 = sysdatetime()
--	insert into [dbo].[Transaction](AccountId, TypeId, CurrencyId, Amount, [Timestamp]) values(@accountId, @typeId, @currencyId, @amount, @timestamp)
--	select scope_identity();
--end

--alter procedure Transaction_AddTransfer
--	@accountId bigint,
--	@typeId tinyint,
--	@currencyId tinyint,
--	@amount money,
--	@destinationLeadId bigint
--as
--begin
--	declare @timestamp datetime2 = sysdatetime()
--	insert into [dbo].[Transaction] (AccountId, TypeId, CurrencyId, Amount, [Timestamp])
--		values (@accountId, @typeId, @currencyId, -@amount, @timestamp)
--		declare @account bigint set @account = scope_identity()
--	insert into [dbo].[Transaction] (AccountId, TypeId, CurrencyId, Amount, [Timestamp])
--		values (@destinationLeadId, @typeId, @currencyId, @amount, @timestamp)
--		select @account as [account]
--		union select scope_identity()
--end

--alter procedure [dbo].[Transaction_GetById]
--@Id bigint
--as
--begin
--    select two.AccountId as TransientAccountId, one.Id, one.AccountId, one.Amount, one.CurrencyId, one.TypeId, one.[Timestamp]
--	from [dbo].[Transaction] as one
--		left outer join [dbo].[Transaction] as two on one.CurrencyId=two.CurrencyId and one.TypeId = two.TypeId
--			and one.[Timestamp]=two.[Timestamp] and one.Amount<>two.Amount and abs(one.Amount)=abs(two.Amount)
--	where one.Id=@Id
--end


--alter procedure [dbo].[Transaction_GetByLeadId]
--  @accountId bigint
--as
--begin
--  select two.AccountId as TransientAccountId, one.Id, one.AccountId, one.Amount, one.CurrencyId, one.TypeId, one.[Timestamp]
--	from [dbo].[Transaction] as one
--		full outer join [dbo].[Transaction] as two on one.CurrencyId=two.CurrencyId and one.TypeId = two.TypeId
--			and one.[Timestamp]=two.[Timestamp] and one.Amount<>two.Amount and abs(one.Amount)=abs(two.Amount)
--	where one.AccountId=@accountId
--end
