USE [master]
CREATE DATABASE [DLock_HandsOn];

USE [DLock_HandsOn];
CREATE TABLE [Resources] (
	Name nvarchar(256) PRIMARY KEY,
	Value nvarchar(max) NOT NULL
);