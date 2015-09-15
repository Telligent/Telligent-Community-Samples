# Telligent Community REST Samples
=========

This project demonstrates very simple examples of using the Telligent Community REST API.

The samples are written to be stand-alone so that you can easily see exactly how API calls are made. They do not demonstrate best practices for interacting with the Telligent Community REST API nor do they represent best coding practices for object oriented coding.

The samples were written with Xamarin Studio and Mono v4.x. Contributions are welcome.

## Setting up the samples
1. You will need to have Telligent Community 8.X or late
2. [Generate an API key](https://community.telligent.com/documentation/w/telligent-community-85/47630.create-an-api-key)
3. Update Main\Rules.config with your API key:

``` xml
	<api>
		<username>[username]</username>
		<apikey>[api key]</apikey>
	</api>
```

4. Update Main\Rules.config with your community URL:

``` xml
	<config pageSize="25" log="true" verbose="false" apiUrl="https://[community url]/api.ashx/v2/">
```