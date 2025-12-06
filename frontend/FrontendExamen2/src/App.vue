<template>
    <div class="coffee-machine">
        <div class="machine-container">

            <h1>Máquina Expendedora de Nacho</h1>

            <div class="layout">
                <div class="main-section">

                    <div class="top-section">
                        <h2>Cafés Disponibles</h2>

                        <table class="coffee-table">
                            <thead>
                                <tr>
                                    <th>Tipo</th>
                                    <th>Precio</th>
                                    <th>Stock</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="([name, qty], index) in coffees" :key="index">
                                    <td>{{ name }}</td>
                                    <td>{{ coffeePrices[name] }}₡</td>
                                    <td>{{ qty }}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="purchase-section">
                        <h2>Comprar Café</h2>

                        <div class="purchase-controls">
                            <label>Tipo:</label>
                            <select v-model="selectedCoffee">
                                <option disabled value="">Seleccione un café</option>
                                <option v-for="([name], index) in coffees" :key="index" :value="name">
                                    {{ name }}
                                </option>
                            </select>

                            <label>Cantidad:</label>
                            <input type="number" min="1" v-model.number="selectedQty" />

                            <button @click="addToCart">Agregar</button>
                        </div>

                        <div v-if="purchaseError" class="error-message">
                            {{ purchaseError }}
                        </div>
                    </div>

                    <div class="payment-section">
                        <h2>Pago</h2>

                        <p><strong>Dinero ingresado:</strong> {{ formatPrice(moneyEntered) }}</p>

                        <button @click="shoSelector = !shoSelector">Añadir fondos</button>

                        <div v-if="shoSelector" class="add-money-box">
                            <select v-model="selectedCoin">
                                <option disabled value="">Seleccione una moneda o billete</option>
                                <option value="25">₡25</option>
                                <option value="50">₡50</option>
                                <option value="100">₡100</option>
                                <option value="500">₡500</option>
                                <option value="1000">₡1000</option>
                            </select>

                            <button @click="addMoney">Agregar</button>
                        </div>


                        <p v-if="pagoError" class="error-message">{{ pagoError }}</p>

                        <button @click="makeTransaction"
                                :disabled="!canMakePurchase"
                                :class="{ 'disabled-btn': !canMakePurchase }">
                            Realizar compra
                        </button>


                    </div>

                </div>

                <aside class="cart-panel">
                    <h3>Carrito</h3>

                    <div v-for="item in cart" :key="item.name" class="cart-item">
                        <strong>{{ item.name }}</strong>
                        <div>Cant: {{ item.qty }}</div>
                        <div>{{ formatPrice(item.total) }}</div>
                    </div>

                    <div class="cart-total">
                        <strong>Total: {{ formatPrice(cartTotal) }}</strong>
                    </div>

                    <p v-if="moneyEntered < cartTotal && moneyEntered > 0"
                       class="error-message">
                        Fondos insuficientes para completar la compra.
                    </p>

                    <div v-if="changeAmount > 0" class="change-box">
                        <h3>Su vuelto es de {{ changeAmount }} colones.</h3>
                        <strong>Desglose:</strong>
                        <ul>
                            <li v-for="(count, denom) in changeBreakdown" :key="denom">
                                {{ count }} moneda{{ count > 1 ? 's' : '' }} de {{ denom }}
                            </li>
                        </ul>
                    </div>
                </aside>

            </div>

            <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>

        </div>
    </div>
</template>


<script>
    import axios from "axios";

    export default {
        name: "CoffeeMachine",

        data() {
            return {
                coffees: [],
                errorMessage: "",
                coffeePrices: "",
                selectedCoffee: "",
                selectedQty: 1,
                purchaseError: "",
                cart: [],
                coins: {},
                coinsError: "",
                insertedCoins: [],
                insertedBills: [],

                moneyEntered: 0,
                returnedMoney: null,
                shoSelector: false,
                selectedCoin: "",
                changeAmount: 0,
                changeBreakdown: {},

            };
        },

        async created() {
            await this.serveApp();
        },

        computed: {
            cartTotal() {
                return this.cart.reduce((acc, item) => acc + item.total, 0);
            },

            canMakePurchase() {
                return (
                    this.cart.length > 0 &&
                    this.moneyEntered > 0 &&
                    this.moneyEntered >= this.cartTotal
                );
            }
        },



        methods: {

            async serveApp() {
                await this.loadCoffees();
                await this.loadCoffeePrices();
                await this.loadCoinsChange();
            },
            async loadCoffees() {
                try {
                    let response = await axios.get("https://localhost:7183/CoffeeData");
                    this.coffees = Object.entries(response.data);
                    console.log(this.coffees);

                } catch (err) {
                    this.errorMessage = "No se pudieron cargar los cafés noouuu";
                    console.error(err);
                }
            },

            showTempError(msg) {
                this.pagoError = msg;

                setTimeout(() => {
                    this.pagoError = "";
                }, 3000);
            },


            async loadCoffeePrices() {
                try {
                    let response = await axios.get("https://localhost:7183/CoffeePrices");
                    this.coffeePrices = response.data;
                    console.log(this.coffeePrices);

                } catch (err) {
                    this.errorMessage = "No se pudo cargar el precio de los cafés noouuu";
                    console.error(err);
                }
            },

            addToCart() {
                this.purchaseError = "";

                if (!this.selectedCoffee) {
                    this.purchaseError = "Debe seleccionar un tipo de café.";
                    return;
                }

                if (!this.selectedQty || this.selectedQty <= 0) {
                    this.purchaseError = "Ingrese una cantidad válida.";
                    return;
                }

                const stock = this.coffees.find(([name]) => name === this.selectedCoffee)[1];

                if (this.selectedQty > stock) {
                    this.purchaseError = "No hay suficiente stock disponible.";
                    return;
                }

                const price = this.coffeePrices[this.selectedCoffee];
                const total = price * this.selectedQty;

                const existing = this.cart.find(i => i.name === this.selectedCoffee);

                if (existing) {
                    existing.qty += this.selectedQty;
                    existing.total = existing.qty * price;
                } else {
                    this.cart.push({
                        name: this.selectedCoffee,
                        qty: this.selectedQty,
                        price: price,
                        total: total
                    });
                }

                this.selectedCoffee = "";
                this.selectedQty = 1;
            },

            async loadCoinsChange() {
                try {
                    let response = await axios.get("https://localhost:7183/CoinsChange");
                    this.coins = response.data;
                    console.log(this.coins);

                } catch (err) {
                    this.coinsError = "No se pudieron cargar las monedas iniciales nooouoou";
                    console.error(err);
                }
            },

            addMoney() {
                if (!this.selectedCoin) return;

                const value = Number(this.selectedCoin);

                this.moneyEntered += value;

                if (value === 1000) {

                    this.insertedBills.push(1000);
                } else {
                    this.insertedCoins.push(value);
                }

                this.selectedCoin = "";
            },

            async updateCoffeeStock() {
                const requests = this.cart.map(item => {
                    const url = `https://localhost:7183/UpdateCoffeeData?coffeName=${encodeURIComponent(item.name)}&count=${item.qty}`;
                    return axios.get(url);
                });

                try {
                    await Promise.all(requests);
                } catch (err) {
                    console.error("Error actualizando el stock: NOOO", err);
                    alert("Hubo un problema actualizando el stock, pero la compra sí se realizó.");
                }
            },

            async makeTransaction() {

                if (this.cart.length === 0) {
                    this.showTempError("El carrito está vacío");
                    return;
                }

                if (this.moneyEntered <= 0) {
                    this.showTempError("El carrito está vacío");
                    return;
                }

                const orderDict = {};
                this.cart.forEach(item => {
                    orderDict[item.name] = item.qty;
                });

                const payload = {
                    order: orderDict,
                    payment: {
                        totalAmount: this.moneyEntered,
                        coins: this.insertedCoins,
                        bills: this.insertedBills
                    }
                };

                try {
                    const response = await axios.post("https://localhost:7183/Transction", payload);
                    console.log("Data que debería de recibir:");
                    console.log(response.data);

                    if (!response.data.success) {
                        alert("La compra no se pudo realizar por falta de fondos: " + response.data.message);
                        return;
                    }

                    alert("La compra se realizó con éxito" + response.data.message);

                    this.resetValues();

                    await this.updateCoffeeStock();

                    await this.updateCoinsAfterPayment();

                    await this.loadCoffees();

                    await this.paymentBreakdown(response.data);

                    this.resetValues();

                } catch (err) {
                    this.pagoError = err.response?.data || "Error procesando el pago.";
                }
            },

            async updateCoinsAfterPayment() {
                const groupedCoins = this.groupCoins(this.insertedCoins);
                const groupedBills = this.groupCoins(this.insertedBills);
                const finalCoins = { ...groupedCoins, ...groupedBills };

                for (const coinValue in finalCoins) {
                    const count = finalCoins[coinValue];

                    try {
                        await axios.get(
                            `https://localhost:7183/CoinsUpdate?coin=${coinValue}&count=${count}`
                        );
                    } catch (error) {
                        console.error("Error actualizando el monedero:", error);
                    }
                }
                await this.loadCoinsChange();
            },

            groupCoins(array) {
                const grouped = {};
                array.forEach(value => {
                    if (!grouped[value]) grouped[value] = 0;
                    grouped[value]++;
                });
                return grouped;
            },

            async paymentBreakdown(breakdownData) {
                this.changeAmount = breakdownData.changeAmount;
                this.changeBreakdown = breakdownData.changeBreakdown;
                setTimeout(() => {
                    this.resetPaymentBreakdown();
                }, 3000);
            },

            resetPaymentBreakdown() {
                this.changeAmount = 0;
                this.changeBreakdown = "";

            },

            resetValues() {

                this.cart = [];
                this.moneyEntered = 0;
                this.insertedBills = [];
                this.insertedCoins = [];
                this.pagoError = "";

            },

            //Profe este método me lo dio la IA pero lo mantuve pq está super útil y no lo sabía hacer
            formatPrice(price) {
                return new Intl.NumberFormat("es-CR", {
                    style: "currency",
                    currency: "CRC",
                    minimumFractionDigits: 0
                }).format(price).replace("CRC", "");
            }
        }
    };
</script>

<style scoped>
    .coffee-machine {
        display: flex;
        justify-content: center;
        padding: 30px;
        font-family: Arial, sans-serif;
    }

    .machine-container {
        width: 780px;
        background: #f3f3f3;
        padding: 25px;
        border-radius: 10px;
        box-shadow: 0 0 12px rgba(0, 0, 0, 0.15);
    }

    .layout {
        display: grid;
        grid-template-columns: 1fr 260px;
        gap: 25px;
    }

    .main-section {
        display: flex;
        flex-direction: column;
        gap: 25px;
    }

    .top-section,
    .purchase-section,
    .payment-section {
        background: white;
        padding: 20px;
        border-radius: 8px;
        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }

    h1 {
        text-align: center;
        margin-bottom: 20px;
    }

    .coffee-table {
        width: 100%;
        border-collapse: collapse;
    }

        .coffee-table th {
            background: #37474f;
            color: white;
            padding: 12px;
        }

        .coffee-table td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
        }

        .coffee-table tr:hover {
            background: #f2f2f2;
        }

    .purchase-controls {
        display: flex;
        gap: 15px;
        align-items: center;
    }

        .purchase-controls select,
        .purchase-controls input {
            padding: 6px 10px;
        }

        .purchase-controls button {
            padding: 8px 14px;
            background: #37474f;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }

            .purchase-controls button:hover {
                background: #455a64;
            }

    .payment-section button {
        margin-top: 10px;
    }

    .pay-button {
        padding: 10px 15px;
        background: #00897b;
        color: white;
        border: none;
        border-radius: 5px;
        margin-left : 10px;
    }

    .cart-panel {
        position: sticky;
        top: 20px;
        height: fit-content;
        background: white;
        padding: 15px;
        border-radius: 10px;
        box-shadow: 0 0 10px rgba(0,0,0,0.15);
    }

    .cart-item {
        margin-bottom: 12px;
        padding-bottom: 6px;
        border-bottom: 1px solid #ddd;
    }

    .cart-total {
        margin-top: 10px;
        font-size: 1.1em;
    }

    .change-box {
        margin-top: 15px;
        padding: 10px;
        background: #e3f2fd;
        border-left: 4px solid #1976d2;
        border-radius: 6px;
    }

    .error-message {
        color: red;
        margin-top: 10px;
    }
    .disabled-btn {
        opacity: 0.5;
        cursor: not-allowed;
    }

</style>
