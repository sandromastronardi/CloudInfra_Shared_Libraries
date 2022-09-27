using System;

namespace CompanyName.Shared.Events.EventSchemas.Storage
{
    internal static class FileNameParser
    {
        internal static (Guid,string) GetMetaData(string file)
        {
            var pathParts = file.Split(new[] { '/', '\\' }, 2);
            Guid jobId = Guid.Empty;
            if (pathParts.Length != 2 || !Guid.TryParse(pathParts[0], out jobId))
            {
                throw new NotSupportedException("The file id for a print job must have a Guid job prefix in its path ({guid}/path.doc)");
            }
            return (jobId, pathParts[1]);
        }
    }
}