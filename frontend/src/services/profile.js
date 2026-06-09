import axiosInstance from "./api";

export async function isUserWithName(){
  const response = await axiosInstance.get('/api/isUserWithName')
  return response.data
}
export async function updateUserFullName(firstName, lastName){
  const data = {
    firstName: firstName,
    lastName: lastName
  }
  await axiosInstance.post('/api/updateUserName', data)
}
export async function getUserName(){
  const response = await axiosInstance.get('/api/getUserName')
  return response.data
}
