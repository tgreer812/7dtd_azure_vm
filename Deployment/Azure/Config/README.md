Copy global_config.template.json to a new file called global_config.json and fill in the values required:

- `vmAdminUsername`: The admin username for the virtual machine
- `vmAdminPassword`: The admin password for the virtual machine 
- `domainNameLabel`: A unique domain name label for the public IP address. This will create a FQDN like `<domainNameLabel>.eastus.cloudapp.azure.com` that you can use to connect to your server instead of remembering the IP address.