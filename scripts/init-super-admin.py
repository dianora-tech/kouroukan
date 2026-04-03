#!/usr/bin/env python3
"""
init-super-admin.py
-------------------
Generates an Argon2id password hash compatible with the C# Argon2PasswordHasher
(Konscious.Security.Cryptography) used by the kouroukan API.

Parameters (must match C# implementation exactly):
  - Type:               Argon2id
  - DegreeOfParallelism: 4
  - MemorySize:          65536 KB (64 MB)
  - Iterations:          3
  - SaltSize:            16 bytes (128 bits)
  - HashSize:            32 bytes (256 bits)

Output format: {salt_base64}:{hash_base64}

Requires: pip install argon2-cffi
"""

import os
import sys
import base64

try:
    from argon2.low_level import hash_secret_raw, Type
except ImportError:
    print(
        "ERROR: argon2-cffi is not installed.\n"
        "Install it with: pip install argon2-cffi",
        file=sys.stderr,
    )
    sys.exit(1)

# ---------------------------------------------------------------------------
# Argon2id parameters — must match Argon2PasswordHasher.cs exactly
# ---------------------------------------------------------------------------
SALT_SIZE = 16               # bytes
HASH_SIZE = 32               # bytes
TIME_COST = 3                # Iterations
MEMORY_COST = 65536          # MemorySize in KB (64 MB)
PARALLELISM = 4              # DegreeOfParallelism
ARGON2_TYPE = Type.ID        # Argon2id


def generate_hash(password: str) -> str:
    """Generate an Argon2id hash in the format expected by the C# backend."""
    salt = os.urandom(SALT_SIZE)

    raw_hash = hash_secret_raw(
        secret=password.encode("utf-8"),
        salt=salt,
        time_cost=TIME_COST,
        memory_cost=MEMORY_COST,
        parallelism=PARALLELISM,
        hash_len=HASH_SIZE,
        type=ARGON2_TYPE,
    )

    salt_b64 = base64.b64encode(salt).decode("ascii")
    hash_b64 = base64.b64encode(raw_hash).decode("ascii")

    return f"{salt_b64}:{hash_b64}"


def main() -> None:
    password = os.environ.get("ADMIN_PASSWORD", "").strip()

    if not password:
        print("ERROR: ADMIN_PASSWORD environment variable is not set or empty.", file=sys.stderr)
        sys.exit(1)

    if len(password) < 12:
        print("ERROR: ADMIN_PASSWORD must be at least 12 characters long.", file=sys.stderr)
        sys.exit(1)

    hashed = generate_hash(password)

    # Output only the hash — the shell script captures this via stdout
    print(hashed)


if __name__ == "__main__":
    main()
