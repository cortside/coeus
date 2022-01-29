using System.Collections.Generic;
using Newtonsoft.Json;

namespace PolicyServer.Mocks.Models
{
    public class Subject
    {
        public string ClientId { get; set; }
        public string SubjectId { get; set; }
        public string UserType { get; set; }
        public string ReferenceToken { get; set; }
        public List<Policy> Policies { get; set; }
    }

    public class Authorization
    {
        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; }
    }

    public class Policy
    {
        public string PolicyName { get; set; }
        public Authorization Authorization { get; set; }
    }

    public class Subjects
    {
        [JsonProperty("subjects")]
        public List<Subject> SubjectsList { get; set; }
    }
}
