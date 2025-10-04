import pyodbc
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import json

app = FastAPI()

# ---------- DB CONNECTION ----------
def get_connection():
    with open("appsettings.json", "r") as f:
        config = json.load(f)

    base_conn = config["connectionStrings"]["DefaultConnection"].strip()

    # Append Driver + Encryption settings
    conn_str = (
        "DRIVER={ODBC Driver 18 for SQL Server};"
        f"{base_conn}"
        "Encrypt=yes;"
        "TrustServerCertificate=yes;"
    )

    # Connect
    conn = pyodbc.connect(conn_str)
    return conn


# ---------- MODELS ----------
class Product(BaseModel):
    productName: str
    price: float
    stock: int

class BuyRequest(BaseModel):
    userId: int
    productId: int
    quantity: int


# ---------- CRUD APIs ----------

@app.get("/products")
def get_all_products():
    conn = get_connection()
    try:
        cursor = conn.cursor()
        cursor.execute("SELECT ProductID, ProductName, Price, Stock FROM Products")
        rows = cursor.fetchall()
        return [
            {"productId": r[0], "productName": r[1], "price": float(r[2]), "stock": r[3]}
            for r in rows
        ]
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        conn.close()


@app.get("/products/{product_id}")
def get_product_by_id(product_id: int):
    conn = get_connection()

    try:
        cursor = conn.cursor()
        cursor.execute("SELECT ProductID, ProductName, Price, Stock FROM Products WHERE ProductID=?", product_id)
        row = cursor.fetchone()
        if not row:
            raise HTTPException(status_code=404, detail="Product not found")
        return {"productId": row[0], "productName": row[1], "price": float(row[2]), "stock": row[3]}
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        conn.close()


@app.post("/products")
def add_product(product: Product):
    conn = get_connection()
    try:
        cursor = conn.cursor()
        cursor.execute(
            "INSERT INTO Products (ProductName, Price, Stock) VALUES (?, ?, ?)",
            (product.productName, product.price, product.stock),
        )
        conn.commit()
        return {"message": "Product added successfully"}
    except Exception as e:
        conn.rollback()
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        conn.close()


@app.put("/products/{product_id}/restock/{quantity}")
def restock_product(product_id: int, quantity: int):
    conn = get_connection()
    try:
        cursor = conn.cursor()
        cursor.execute("UPDATE Products SET Stock = Stock + ? WHERE ProductID = ?", (quantity, product_id))
        if cursor.rowcount == 0:
            raise HTTPException(status_code=404, detail="Product not found")
        conn.commit()
        return {"message": f"Product {product_id} restocked by {quantity}"}
    except HTTPException:
        raise
    except Exception as e:
        conn.rollback()
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        conn.close()


@app.delete("/products/{product_id}")
def delete_product(product_id: int):
    conn = get_connection()
    try:
        cursor = conn.cursor()
        cursor.execute("DELETE FROM Products WHERE ProductID=?", product_id)
        if cursor.rowcount == 0:
            raise HTTPException(status_code=404, detail="Product not found")
        conn.commit()
        return {"message": "Product deleted successfully"}
    except HTTPException:
        raise
    except Exception as e:
        conn.rollback()
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        conn.close()


# ---------- BUY PRODUCT USING SP ----------
@app.post("/buy")
def buy_product(request: BuyRequest):
    conn = get_connection()
    cursor = conn.cursor()
    try:
        cursor.execute("EXEC sp_BuyProduct @UserID=?, @ProductID=?, @Quantity=?",
                       (request.userId, request.productId, request.quantity))
        conn.commit()
    except Exception as e:
        conn.rollback()
        conn.close()
        raise HTTPException(status_code=400, detail=str(e))
    conn.close()
    return {"message": f"User {request.userId} bought {request.quantity} of product {request.productId}"}


@app.put("/orders/{bought_id}/cancel")
def cancel_order(bought_id: int):
    conn = get_connection()
    cursor = conn.cursor()
    try:
        # Call the stored procedure
        cursor.execute("EXEC sp_CancelOrder @BoughtID=?", (bought_id,))
        
        # Try fetching the updated row (the SP returns SELECT)
        row = cursor.fetchone()
        if not row:
            raise HTTPException(status_code=404, detail="Order not found or already cancelled")

        # Map columns to dictionary
        columns = [col[0] for col in cursor.description]
        result = dict(zip(columns, row))

        conn.commit()
    except Exception as e:
        conn.rollback()
        conn.close()
        raise HTTPException(status_code=400, detail=str(e))
    conn.close()

    return result
