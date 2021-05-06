using ProjectX.Core;
using System;
using System.IO;

namespace ProjectX.FileStorage.Application
{
    public sealed class DownloadFileQuery : IQuery<DownloadFileQuery.Response>
    {
        public Guid Id { get; }

        public DownloadFileQuery(Guid id)
        {
            Id = id;
        }

        public sealed class Response 
        {
            public Response(Stream file, string mimeType)
            {
                File = file;
                MimeType = mimeType;
            }

            public Stream File { get; }
            public string MimeType { get; }
        }
    }
}
