import axiosInstance from "./api";

export async function register(email, password){
  const data = {
      email,
      password
  }
  const response = await axiosInstance.post("/register", data)
  return response.data
}

export async function login(email, password){
  const data = {
      email,
      password
  }
  const response = await axiosInstance.post("/login?useCookies=true", data)
  return response.data
}