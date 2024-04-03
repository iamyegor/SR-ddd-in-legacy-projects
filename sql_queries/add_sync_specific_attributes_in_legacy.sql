create table Synchronization (
	Name nvarchar(100) primary key,
	IsSyncRequired bit not null default 0,
	RowVersion rowversion
)
go

insert into Synchronization (Name)
values ('Product')
go

alter table PRD_TBL
add IsSyncNeeded bit not null default 0
go

create trigger ProductSync on PRD_TBL
after update, insert
as
begin
	set nocount on;

	if (update(NM_CLM) or update(WT) or update(WT_KG))
	begin
        update PRD_TBL
	    set IsSyncNeeded  = 1
	    where NMB_CM in (select NMB_CM from inserted)

	    update Synchronization
	    set IsSyncRequired = 1
	    where Name = 'Product'
    end
end