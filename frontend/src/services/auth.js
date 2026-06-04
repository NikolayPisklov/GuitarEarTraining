import instance from "./api";

export async function register(email, password){
    const data = {
        email,
        password
    }
    const response = await instance.post("/register", data)
    return response.data
}