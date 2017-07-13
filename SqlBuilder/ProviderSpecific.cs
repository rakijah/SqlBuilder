using System;

namespace SqlBuilder
{
    /// <summary>
    /// Provides provider-specific SQL strings.
    /// </summary>
    public static class ProviderSpecific
    {
        /// <summary>
        /// The column type to be used for dates (or datetimes) for this provider.
        /// </summary>
        public static string DateType
        {
            get
            {
                switch (SqlBuild.Provider)
                {
                    case DatabaseProvider.SQLite:
                        return "TEXT";

                    default:
                        return "DATETIME";
                }
            }
        }

        /// <summary>
        /// A function that can be used in an SQL statement to convert the provided dateValue to a database-internal date.
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public static string DateInsertFunction(string dateValue)
        {
            switch (SqlBuild.Provider)
            {
                case DatabaseProvider.SQLite:
                    return $"datetime('{dateValue}')";

                case DatabaseProvider.Oracle10GOrLater:
                    return $"TO_DATE('{dateValue}', '{InternalDateFormatString}')";

                default:
                    throw new NotImplementedException("This provider has not been fully implemented yet. Sorry!");
            }
        }

        /// <summary>
        /// The database-internal date represenation format.<para />
        /// These strings should be used inside SQL statement strings.
        /// </summary>
        public static string InternalDateFormatString
        {
            get
            {
                switch (SqlBuild.Provider)
                {
                    case DatabaseProvider.SQLite:
                        return "YYYY-MM-DD HH:MM:SS";

                    case DatabaseProvider.Oracle10GOrLater:
                        return $"YYYY-MM-DD HH24:MI:SS";

                    default:
                        throw new NotImplementedException("This provider has not been fully implemented yet. Sorry!");
                }
            }
        }

        /// <summary>
        /// The provider-specific date format string to be used with DateTime.ParseExact().
        /// </summary>
        public static string DotNetDateFormatString
        {
            get
            {
                switch (SqlBuild.Provider)
                {
                    default:
                        return "yyyy-MM-dd hh:mm:ss";
                }
            }
        }
    }
}