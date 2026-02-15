using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static LifeAdminModels.GCommons.NotificationKeys;

namespace LifeAdmin.Infrastructure;

public static class TempDataExtensions
{
    public static void SetSuccess(this ITempDataDictionary tempData, string message)
        => tempData[SuccessMessage] = message;

    public static void SetError(this ITempDataDictionary tempData, string message)
        => tempData[ErrorMessage] = message;

    public static void SetWarning(this ITempDataDictionary tempData, string message)
        => tempData[WarningMessage] = message;

    public static void SetInfo(this ITempDataDictionary tempData, string message)
        => tempData[InfoMessage] = message;
}
