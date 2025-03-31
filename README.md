# 🚀 Cómo ejecutar 

1. 🛠️ Clona el repositorio:
```
git clone https://github.com/FrancoBrice/order-management-api.git
cd OrderManagementAPI
```

2. 🔑 Crea un archivo .env con esta variable:
```
SA_PASSWORD=YourStrongPassw0rd
```

3. 🐳 Ejecuta el contenedor Docker:
```
docker-compose up --build
```

4. 🌐 Abre en tu navegador: 
```
http://localhost:5000/swagger
```
Aquí podrás ver la documentación de la API y probar los endpoints.  Cómo ejecutar 

## 🧪 Pruebas con Postman

Puedes probar los endpoints desde Postman o cualquier cliente HTTP. A continuación, algunos ejemplos:

### Productos
- GET /productos: Lista todos los productos.

- POST /productos: Crea un nuevo producto. Ejemplo:

```json
{
  "nombre": "Brownie",
  "precio": 300
}
```

### Órdenes
Antes de crear órdenes debes crear productos en la base de datos con el endpoint `POST /productos`.

- `GET /ordenes`: Obtiene todas las órdenes de compra (soporta paginación con `?pageNumber=1&pageSize=10`).
- `GET /ordenes/{id}`: Obtiene una orden específica por su ID.
- `POST /ordenes`: Crea una nueva orden. Envia un JSON como este:

```json
{
  "cliente": "Juan Pérez",
  "fechaCreacion": "2025-03-28T14:00:00",
  "orderProducts": [
    { "productId": 1, "quantity": 2 },
    { "productId": 2, "quantity": 1 }
  ]
}
```

- PUT /ordenes/{id}: Actualiza una orden existente. Debes enviar el id también en el body:

```json
{
  "id": 1001,
  "cliente": "Cliente Actualizado",
  "fechaCreacion": "2025-03-28T14:00:00",
  "orderProducts": [
    { "productId": 1, "quantity": 1 },
    { "productId": 3, "quantity": 2 }
  ]
}
```
- DELETE /ordenes/{id}: Elimina una orden por su ID.

