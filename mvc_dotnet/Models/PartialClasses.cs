using Microsoft.AspNetCore.Mvc;
using mvc_dotnet.Models.Metadata;
using System.ComponentModel.DataAnnotations;


namespace mvc_dotnet.Models
{       
    [ModelMetadataType(typeof(UserMetadata))]
    public partial class User
    {

    }

    [ModelMetadataType(typeof(AdminMetadata))]
    public partial class Admin
    {

    }

    [ModelMetadataType(typeof(ContentAdminMetadata))]
    public partial class ContentAdmin
    {

    }

    [ModelMetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {

    }

    [ModelMetadataType(typeof(ProvoleMetadata))]
    public partial class Provole
    {

    }

    [ModelMetadataType(typeof(CreateProvoleMetadata))]
    public partial class CreateProvoleModel
    {

    }



    [ModelMetadataType(typeof(MovieMetadata))]
    public partial class Movie
    {

    }

    [ModelMetadataType(typeof(ReservationMetadata))]
    public partial class Reservation
    {

    }


    }

