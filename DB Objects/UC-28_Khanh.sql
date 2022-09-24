alter table Rooms 
add [Name] varchar(50) null, [Capacity] int null

alter table Classes 
add [Name] varchar(50) null, [Enrollments] int null

alter table [Time] 
add [StartTime] time(0) null, [EndTime] time(0) null

alter table [Time]
add [TimeID] int IDENTITY(1,1) not null Primary key


alter table Weekdays
alter column [WeekdayID] varchar(50) not null

alter table Rooms 
add [RoomID] int IDENTITY(1,1) not null primary key

alter table schedules 
drop constraint FK_Schedule_Room

insert into Lectures 
values (3720, NULL)

insert into Rooms 
values (276, NULL, 'Peter Kiewit Institute 276', 30)

select * from Rooms

update Rooms 
set Name = 'Peter Kiewit Institute 276', Capacity = 30
where RoomID = 276

declare @startTime time(0) = '13:30:00',
		@endTime time(0) = '14:45:00'

select @startTime, @endTime

insert into [Time]
values (@startTime, @endTime)


select * from Rooms

alter table Schedules 
add [ScheduleID] int Identity(1,1) not null primary key