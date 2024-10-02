﻿global using EFCore.BulkExtensions;
global using FluentValidation;
global using LinqKit;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using NetCoreWebApiPrintPDF.Application.Features.Employees.Queries.GetEmployees;
global using NetCoreWebApiPrintPDF.Application.Features.Positions.Queries.GetPositions;
global using NetCoreWebApiPrintPDF.Application.Interfaces;
global using NetCoreWebApiPrintPDF.Application.Interfaces.Repositories;
global using NetCoreWebApiPrintPDF.Application.Parameters;
global using NetCoreWebApiPrintPDF.Domain.Common;
global using NetCoreWebApiPrintPDF.Domain.Entities;
global using NetCoreWebApiPrintPDF.Infrastructure.Persistence.Contexts;
global using NetCoreWebApiPrintPDF.Infrastructure.Persistence.Repository;
global using NetCoreWebApiPrintPDF.Infrastructure.Shared.Services;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Linq.Dynamic.Core;
global using System.Threading;
global using System.Threading.Tasks;