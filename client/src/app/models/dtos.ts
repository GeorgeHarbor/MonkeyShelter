export interface UserLoginDto {
  username: string;
  password: string;
}

export interface UserRegisterDto {
  username: string;
  email: string;
  password: string;
  shelterId: string;
}

export interface AuthResponse {
  token: string;
  userName: string;
  email: string;
  shelterId: string;
  id: string;
}
