# FastCommerce

This is an e-commerce project developed with Blazor and .NET 8. The project includes a catalog API, an API gateway using Ocelot, and a Blazor WebAssembly client application.

Fast Architecture:
![image](https://drive.google.com/uc?export=view&id=1UERJQpkr7gxBEl_z1EBlpyxlVE55nTxS)


<!-- ROADMAP -->
## Roadmap

- [x] Net. 8
- [x] API Gateway - <a href="https://github.com/ThreeMammals/Ocelot">Ocelot</a>
- [X] ORM - <a href="https://github.com/DapperLib/Dapper">Dapper</a>
- [ ] RabbitMQ
- [X] Authentication
- [ ] Payment - <a href="https://app.openpix.com/">OpenPix</a>
- [ ] Authorization ACL
- [X] CQRS Patern Generic
- [ ] Webhook Client
- [ ] Multi-language Support
    - [ ] Chinese
    - [ ] Spanish
    - [ ] English


## Requirements

To run this project, you will need to have the following software installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (with Linux support)

## Environment Setup

1. **Clone the repository:**
    git clone https://github.com/ct0rm4n/FastCommerce.git cd fastcommerce

2. **Ensure Docker is running:**
   Verify that Docker is installed and running on your system. You can check this by running the following command:
   docker --version   

3. **Build and start the Docker containers:**
   Navigate to the root directory of the project and run the command below to build and start the Docker containers:
   docker-compose up
   This command will build the necessary Docker images and start the containers defined in the `docker-compose.yml` file.

## Project Structure

- **Api.Ocelot**: API gateway using Ocelot.
- **FastCommerce.Api.Catalog**: Catalog API for managing products and categories.
- **UI**: Blazor WebAssembly client application.

## Accessing the Application

After running the `docker compose up` command, you can access the application at the following URLs:

- **API Gateway**: [http://host.docker.internal:5000/swagger/index.html](http://host.docker.internal:5000/swagger/index.html)
- **Blazor Client Application**: [http://localhost:5005](http://host.docker.internal:5005)

## SQL Scripts

The project includes an SQL script (`CreateDatabase.sql`) to create the database and necessary tables. This script is automatically executed when the catalog API starts, ensuring the database is properly configured.

## Contribution

If you would like to contribute to this project, feel free to open a pull request or report issues in the repository.

## License
This project is licensed under the [MIT License](LICENSE).

    

