﻿global using AutoBogus;
global using Bogus;
global using Bogus.DataSets;
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using MimeKit;
global using NetCoreWebApiPrintPDF.Application.DTOs.Email;
global using NetCoreWebApiPrintPDF.Application.Exceptions;
global using NetCoreWebApiPrintPDF.Application.Interfaces;
global using NetCoreWebApiPrintPDF.Domain.Entities;
global using NetCoreWebApiPrintPDF.Domain.Enums;
global using NetCoreWebApiPrintPDF.Domain.Settings;
global using NetCoreWebApiPrintPDF.Infrastructure.Shared.Mock;
global using NetCoreWebApiPrintPDF.Infrastructure.Shared.Services;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;