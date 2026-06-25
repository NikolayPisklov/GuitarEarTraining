import axiosInstance from "./api";

export async function isUserWithName(){
  const response = await axiosInstance.get('/profile/is-user-with-name')
  return response.data
}
export async function updateUserFullName(firstName, lastName){
  const data = {
    firstName: firstName,
    lastName: lastName
  }
  await axiosInstance.post('/profile/update-user-name', data)
}
export async function getUserName(){
  const response = await axiosInstance.get('/profile/get-user-name')
  return response.data
}
