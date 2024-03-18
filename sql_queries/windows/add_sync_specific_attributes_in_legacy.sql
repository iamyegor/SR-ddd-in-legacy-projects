create table sync (
	name nvarchar(100) primary key,
	is_sync_required bit not null default 0,
	row_version rowversion
)
go

insert into sync (Name)
values ('Product')
go

alter table PRD_TBL
add is_sync_needed bit not null default 0
go

create trigger ProductSync on PRD_TBL
after update, insert
as
begin
	set nocount on;

	if (update(NM_CLM) or update(WT) or update(WT_KG))
	begin
        update PRD_TBL
	    set is_sync_needed  = 1
	    where NMB_CM in (select NMB_CM from inserted)

	    update Synchronization
	    set is_sync_required = 1
	    where Name = 'Product'
    end
end