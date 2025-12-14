using System.Text.Json.Serialization;

namespace Healthcare.Core.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Admin,
        OrganizationHead,
        Provider, // Doctors
        Clinical, // Nurses
        Lab,      // Technicians
        Pharmacy, // Pharmacists
        Reception, // Front Desk
        Billing,   // Finance
        Support,   // IT, BioMed
        Patient
    }
}
