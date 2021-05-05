import { InjectionToken } from "@angular/core";

export const AUTH_API_URL = new InjectionToken<string>('Identity API URL');
export const USERS_API_URL = new InjectionToken<string>('Users API URL');
export const BLOG_API_URL = new InjectionToken<string>('Blog API URL');