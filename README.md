# Prueba técnica para Backend


Disponemos del esqueleto de una API con una arquitectura por capas con un CRUD (casi) funcional.
Lee el siguiente enunciado, y a continuación, los resultados que esperamos encontrar una vez entregada esta prueba técnica.
La API incluye un Swagger que mapea los controllers y sus EndPoints, puedes realizar las pruebas del CRUD desde el propio Swagger, pero siéntete libre de hacerlo desde Postman o cualquier otro entorno en el que te encuentres cómodo.

## _Enunciado_

Pero... ¡Un Junior nos ha borrado una pequeña parte del código por error! Por suerte hemos podido encontrar dónde se ubicaba este código, y hemos dejado algunos comentarios con TODOs por el código para que nos lo puedas rellenar y volver a hacer funcionar.

Por lo que hemos podido apreciar, se ha estropeado la inyección de dependencias del DL, algunos métodos del AppService (BL), otras pocas líneas del Repository (DL), entre otras cositas...
Si abres el "Task List" de VisualStudio o buscas por TODO en toda la solución, podrás encontrar todas las cosas a corregir.

La serialización de datos de un cliente, se vería reflejada tal que así:
```sh
{
	"id" : 12345,
	"docType" : "nif",
	"docNum" : "11223344E",
	"email" : "eromani@sample.com",
	"givenName" : "Enriqueta",
	"familyName1" : "Romani",
	"phone" : "668668668"
}
```

Si puedes dejar el CRUD funcionando (tal y como estaba antes del pequeño incidente...), ya nos habrás ayudado bastante!

Una vez te funcione el CRUD, estaría bien que pudieses implementar las siguientes mejoras al CRUD de clientes:
1. Comprobar que no se repita el DocNum con ningún otro usuario cuando se inserta (además del Id).
2. Validar que DocNum cumple el formato de NIF (8 dígitos y 1 letra) con un regex cuando el DocType es "nif". (Solo cuando es NIF, ignorar formato de NIE, CIF, etc.)
3. Implementar inversión de dependencias en la inyección del Repository en el AppService.
4. Refactorizar los nombres de las variables de ClientAppService. (El Junior estaba inspirado el día que creó esta clase...)

Necesitaríamos, que una vez tengas funcionando este CRUD, nos hagas uno exactamente igual para la entidad de Producto.
La serialización de datos de un producto, se vería reflejada tal que así:
```sh
{
	"id" : 1111111,
	"productName" : "Cristal ventanilla delantera",
	"productType" : 25,
	"numTerminal" : 933933933,
	"soldAt" : "2019-01-09 14:26:17"
}
```

Con que crees las validaciones para Id y ProductName, es más que suficiente.

Además, nos gustaría poder disponer de un controller llamado "HealthCheck", que tenga un EndPoint accesible con método Get cuyo nombre sea "KeepAlive", que simplemente devolverá un 200 OK, donde tendremos un servicio (que no debes implementar) que irá consultando que la API esté levantada en todo momento.

## _Resultados a evaluar_
1. Has podido intuir y rellenar el código que faltaba para que funcionase el CRUD de clientes.
2. Has podido implementar las mejoras (o algunas de ellas) solicitadas en el CRUD de clientes.
3. Has utilizado un buen naming convention en el refactor de ClientAppService.
4. Has podido implementar el CRUD de productos, basándote en el de clientes.
5. Has podido crear el controller de HealthCheck, con el EndPoint correspondiente.

## _Puntos extra_
1. Crear proyecto de Unit Testing donde pruebes las funciones CRUD.
2. Conseguir inyección de dependencias de validadores de forma genérica. (Parecido a la inyección de AppServices del Module de BL)
3. Coméntanos si has encontrado código mejorable en la prueba. (No cumple naming convention, se incumple algún principio SOLID, hay algo mal estructurado, etc.)