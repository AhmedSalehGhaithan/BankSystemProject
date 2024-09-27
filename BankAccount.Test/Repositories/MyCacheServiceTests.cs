using BankSystem.Application.Interface;
using BankSystem.Infrastructure.Repositories;
using Moq;
using System;
using System.Runtime.Caching;
using Xunit;

public class MyCacheServiceTests
{
    private readonly MyCacheService _cacheService;
    private readonly ObjectCache _memoryCache;

    public MyCacheServiceTests()
    {
        _memoryCache = MemoryCache.Default;
        _cacheService = new MyCacheService();
    }

    [Fact]
    public void GetData_ShouldReturnCachedData_WhenKeyExists()
    {
        // Arrange
        string key = "testKey";
        string expectedValue = "testValue";
        _memoryCache.Set(key, expectedValue, DateTimeOffset.UtcNow.AddMinutes(10));

        // Act
        var result = _cacheService.GetData<string>(key);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void GetData_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        // Arrange
        string key = "nonExistentKey";

        // Act
        var result = _cacheService.GetData<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SetData_ShouldStoreData_WhenKeyIsValid()
    {
        // Arrange
        string key = "testKey";
        string value = "testValue";
        DateTimeOffset expirationDate = DateTimeOffset.UtcNow.AddMinutes(10);

        // Act
        var result = _cacheService.SetData(key, value, expirationDate);

        // Assert
        Assert.True(result);
        Assert.Equal(value, _memoryCache.Get(key));
    }

    [Fact]
    public void SetData_ShouldReturnFalse_WhenKeyIsNull()
    {
        // Arrange
        string key = null;
        string value = "testValue";
        DateTimeOffset expirationDate = DateTimeOffset.UtcNow.AddMinutes(10);

        // Act
        var result = _cacheService.SetData(key, value, expirationDate);

        // Assert
        Assert.False(result);
    }
}
