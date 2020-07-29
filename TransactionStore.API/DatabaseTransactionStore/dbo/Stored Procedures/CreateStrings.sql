CREATE procedure CreateStrings
@rowValue bigint 
as
	begin
		declare @length int = 0
		while @length < @rowValue
		begin
		declare @leadId_1 bigint
        declare @leadId_2 bigint
		declare @typeId tinyint
		declare @currencyId tinyint
		declare @amountDeposit money
		declare @timestamp datetime2
		declare @amountWithdraw money

		set @leadId_1=(select round((rand()*100000),1))

		set @leadId_2=@leadId_1+1

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
					LeadId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp])
					values (@leadId_1,
							@typeId,
							@currencyId,
							@amountDeposit,
							@timestamp)
		end

		if @typeId=2
			begin
				insert into [Transaction](
					LeadId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp])
					values (@leadId_1,
							@typeId,
							@currencyId,
							@amountWithdraw,
							@timestamp)
		end

		if @typeId=3
			begin
				insert into [Transaction](
					LeadId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp])
					values (@leadId_1,
							@typeId,
							@currencyId,
							@amountDeposit,
							@timestamp)
				insert into [Transaction](
					LeadId,
					TypeId,
					CurrencyId,
					Amount,
					[Timestamp])
					values (@leadId_2,
							@typeId,
							@currencyId,
							@amountWithdraw,
							@timestamp)
		end
	set @length = @length+1
	end
end